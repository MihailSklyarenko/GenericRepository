using System.Collections.Generic;

namespace Tests.Infrastructure.Database.Entities;

public class TestCompany
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int? CityId { get; set; }
    public TestCity? City { get; set; }

    public ICollection<TestUser>? Users { get; set; }
}