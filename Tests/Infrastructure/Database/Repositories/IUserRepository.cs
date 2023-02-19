using GenericRepository.Interfaces;
using Tests.Infrastructure.Database.Entities;

namespace Tests.Infrastructure.Database.Repositories;

internal interface IUserRepository : IRepository<TestUser, int>
{
}
