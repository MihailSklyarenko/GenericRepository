using GenericRepository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tests.Infrastructure.Database.Contexts;
using Tests.Infrastructure.Database.Repositories;
using Tests.Infrastructure.DependencyInjection.Interfaces;

namespace Tests.Infrastructure.DependencyInjection.Implementation
{
    public class Startup : IDependencyRegistrator
    {
        public IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();

            RegisterDatabase(services);

            return services.BuildServiceProvider();
        }

        private void RegisterDatabase(IServiceCollection services)
        {
            services.AddDbContext<TestDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlite(new SqliteConnectionStringBuilder
                {
                    Mode = SqliteOpenMode.Memory
                }.ConnectionString);
            });

            services.AddScoped(typeof(UserRepository));
        }
    }
}
