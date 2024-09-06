using Microsoft.EntityFrameworkCore;
using Projekt.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Data.Repositories
{
    public class CountriesRepository : IRepository<Country>
    {
        private readonly DataContext context;

        public CountriesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task Add(Country newCountry)
        {
            var country = await context.Countries.FirstOrDefaultAsync(x => x.Name == newCountry.Name);
            if(country != null)
            {
                return;
            }
            await context.Countries.AddAsync(newCountry);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Country>> Get(Func<Country, bool> condition)
        {
            return context.Countries.Where(condition).AsEnumerable();
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            return await context.Countries.ToListAsync();
        }
    }
}
