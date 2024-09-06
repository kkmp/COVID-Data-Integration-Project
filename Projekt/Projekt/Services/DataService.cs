using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Projekt.Data;
using Projekt.Data.Models;
using Projekt.Data.Repositories;
using Projekt.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Projekt.Services
{
    public class DataService : IHostedService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public DataService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var countriesRepository = scope.ServiceProvider.GetRequiredService<IRepository<Country>>();

            var data = await JsonData.GetDailyCases(httpClient);

            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in data)
                {
                    var country = new Country
                    {
                        Id = item.ID,
                        Code = item.CountryCode,
                        Name = item.Country,
                        Slug = item.Slug
                    };
                    await countriesRepository.Add(country);
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
