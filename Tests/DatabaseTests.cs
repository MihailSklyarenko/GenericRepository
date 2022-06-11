using GenericRepository.Enums;
using GenericRepository.Enums.Sorting;
using GenericRepository.Models.Sorting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IUserRepository _repository;

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
            _repository = ServiceProvider.GetRequiredService<IUserRepository>();
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

            var user = await _repository.FirstOrDefaultAsync(x => x.Name == "test1");

            Assert.True(user.Id == 1);

            TestUser[] users =
            {
                new TestUser
                {
                    Id = 2, Name = "test2"
                },
                new TestUser
                {
                    Id = 3, Name = "test3"
                }
            };

            await _repository.AddAsync(users);
            await _repository.SaveChangesAsync();

            var allUsers = await _repository.SelectByConditionAsync(x => x.Id > 0);

            Assert.Collection(allUsers,
                x => Assert.Equal("test1", x.Name),
                x => Assert.Equal("test2", x.Name),
                x => Assert.Equal("test3", x.Name));
        }

        [Fact]
        public async Task Select_Entity_Success()
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

            await _repository.AddAsync(users);
            await _repository.SaveChangesAsync();

            var sorting = new List<SortingParameter>()
               {
                   new SortingParameter(nameof(TestUser.Name), SortDirection.Descending)
               };

            var allUsers = await _repository.SelectByConditionAsync(x => x.Id > 0, sorting);

            Assert.Collection(allUsers,
                x => Assert.Equal("test2", x.Name),
                x => Assert.Equal("test1", x.Name));

            var userWithId1 = await _repository.SingleOrDefaultAsync(x => x.Id == 1);
            Assert.True(userWithId1.Id == 1);
        }

        [Fact]
        public async Task Aggregate_Functions_Success()
        {
            var anyUsers = await _repository.AnyAsync();
            Assert.False(anyUsers);

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

            await _repository.AddAsync(users);
            await _repository.SaveChangesAsync();

            var userWithId1 = await _repository.AnyAsync(x => x.Id == 1);
            var userWithId7 = await _repository.AnyAsync(x => x.Id == 7);
            Assert.True(userWithId1);
            Assert.False(userWithId7);

            var usersCount = await _repository.CountAsync();
            Assert.True(usersCount == 2);

            var usersTest2Count = await _repository.CountAsync(x => x.Name == "test2");
            Assert.True(usersTest2Count == 1);

            await _repository.AddAsync(new TestUser()
            {
                Id = 3,
                Name = "demo"
            });
            await _repository.SaveChangesAsync();

            var userDemo = await _repository.AnyAsync(x => x.Name == "demo");
            Assert.True(userDemo);
            var userblahBlah = await _repository.AnyAsync(x => x.Name == "blahBlah");
            Assert.False(userblahBlah);
        }
    }
}