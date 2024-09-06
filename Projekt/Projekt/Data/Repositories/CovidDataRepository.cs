using Microsoft.EntityFrameworkCore;
using Projekt.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Data.Repositories
{
    public class CovidDataRepository : IRepository<DailyCase>
    {
        private readonly DataContext context;

        public CovidDataRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task Add(DailyCase dailyCase)
        {
            var oldCase = await context.DailyCases.FirstOrDefaultAsync(x => x.Date.Date == dailyCase.Date.Date && x.Country.Name == dailyCase.Country.Name);
            if(oldCase != null)
            {
                return;
            }
            var country = await context.Countries.FirstOrDefaultAsync(x => x.Name == dailyCase.Country.Name);
            if(country == null)
            {
                return;
            }
            dailyCase.Country = country;
            await context.DailyCases.AddAsync(dailyCase);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DailyCase>> Get(Func<DailyCase, bool> condition)
        {
            return context.DailyCases.Include(x => x.Country).Where(condition).AsEnumerable();
        }

        public async Task<IEnumerable<DailyCase>> GetAll()
        {
            return await context.DailyCases.Include(x => x.Country).ToListAsync();
        }
    }
}
