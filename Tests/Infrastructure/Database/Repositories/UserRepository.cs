﻿using GenericRepository;
using Tests.Infrastructure.Database.Contexts;
using Tests.Infrastructure.Database.Entities;

namespace Tests.Infrastructure.Database.Repositories
{
    public class UserRepository : Repository<TestUser>, IUserRepository
    {
        public UserRepository(TestDbContext context) : base(context)
        {
        }
    }
}
