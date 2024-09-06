using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt.Data;
using Projekt.Data.Models;
using Projekt.Data.Repositories;
using Projekt.Json;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataController : ControllerBase
    {
        private readonly IRepository<DailyCase> dataRepository;
        private readonly IRepository<Country> countriesRepository;
        private readonly HttpClient httpClient;
        private readonly DataContext context;

        public DataController(IRepository<DailyCase> dataRepository,
            IRepository<Country> countriesRepository,
            HttpClient httpClient,
            DataContext context)
        {
            this.dataRepository = dataRepository;
            this.countriesRepository = countriesRepository;
            this.httpClient = httpClient;
            this.context = context;
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> Filter(Guid countryId, DateTime dateFrom, DateTime dateTo)
        {
            var country = (await countriesRepository.Get(x => x.Id == countryId)).FirstOrDefault();
            if (country == null)
            {
                return BadRequest("Country not found");
            }

            var data = await JsonData.GetTotalDailyCasesByCountryAsync(httpClient, country.Name);

            var transaction = await context.Database.BeginTransactionAsync();
            foreach (var item in data)
            {
                var dailyCase = new DailyCase
                {
                    Country = country,
                    Date = item.Date.Value,
                    NewConfirmed = item.Confirmed,
                    NewDeaths = item.Deaths,
                    NewRecovered = item.Recovered
                };
                await dataRepository.Add(dailyCase);
            }
            await transaction.CommitAsync();

            var result = await dataRepository.Get(x => x.Country.Id == countryId && x.Date.Date >= dateFrom.Date && x.Date.Date <= dateTo.Date);
            return Ok(result);
        }

        [HttpGet("Countries")]
        public async Task<IActionResult> Countries()
        {
            return Ok(await countriesRepository.GetAll());
        }
    }
}
