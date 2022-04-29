using Microsoft.EntityFrameworkCore;
using Tests.Infrastructure.Database.Entities;

namespace Tests.Infrastructure.Database.Contexts
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() { }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestUser>();
            modelBuilder.Entity<TestCompany>();
            modelBuilder.Entity<TestCity>();
        }
    }
}
