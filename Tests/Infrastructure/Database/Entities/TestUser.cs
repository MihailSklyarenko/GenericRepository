namespace Tests.Infrastructure.Database.Entities
{
    public class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? CompanyId { get; set; }
        public TestCompany? Company { get; set; }
    }
}
