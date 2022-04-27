using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tests.Infrastructure.BaseTest;
using Tests.Infrastructure.Database.Contexts;
using Tests.Infrastructure.Database.Entities;
using Tests.Infrastructure.Database.Repositories;
using Tests.Infrastructure.DependencyInjection.Implementation;
using Xunit;

namespace Tests
{
    public class DatabaseTests : BaseTest<Startup>
    {
        private readonly TestDbContext _context;
        private readonly UserRepository _repository;

        public override async Task InitializeAsync()
        {
            if (_context.Database.IsSqlite())
            {
                await _context.Database.OpenConnectionAsync();
                await _context.Database.EnsureCreatedAsync();
            }
        }

        public DatabaseTests()
        {
            _context = ServiceProvider.GetRequiredService<TestDbContext>();
            _repository = ServiceProvider.GetRequiredService<UserRepository>();
        }

        [Fact]
        public async Task Create_Entity_Success()
        {
            var entity = new TestUser 
            { 
                Id = 1, 
                Name = "test1" 
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var user = await _repository.FirstOrDefaultAsync<TestUser>(x => x.Name == "test1");

            Assert.True(user.Id == 1);
        }

        [Fact]
        public async Task Create_RangeEntitys_Success()
        {
            TestUser[] users = 
            {
                new TestUser
                { 
                    Id = 1, Name = "test1" 
                },
                new TestUser
                {
                    Id = 2, Name = "test2"
                }
            };

            await _repository.AddAsync<TestUser>(users);
            await _repository.SaveChangesAsync();

            var allUsers = await _repository.SelectByConditionAsync<TestUser>(x => x.Id > 0);

            Assert.Collection(users,
                x => Assert.Equal("test1", x.Name),
                x => Assert.Equal("test2", x.Name));
        }
    }
}