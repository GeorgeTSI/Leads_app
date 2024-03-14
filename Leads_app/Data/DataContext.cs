using Leads_app.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Leads_app.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> option) : base(option)
        {
        }

        public DbSet<Leads> Leads { get; set; }
        public DbSet<BasicUsers> BasicUsers { get; set; }
    }
}
