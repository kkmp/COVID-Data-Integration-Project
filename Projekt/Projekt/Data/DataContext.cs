using Microsoft.EntityFrameworkCore;
using Projekt.Data.Models;

namespace Projekt.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<DailyCase> DailyCases { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
