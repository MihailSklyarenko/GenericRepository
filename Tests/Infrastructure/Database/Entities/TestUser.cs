using GenericRepository.Entities;

namespace Tests.Infrastructure.Database.Entities
{
    public class TestUser : BaseEntity<int>
    {
        public string Name { get; set; }

        public int? CompanyId { get; set; }
        public TestCompany? Company { get; set; }
    }
}
