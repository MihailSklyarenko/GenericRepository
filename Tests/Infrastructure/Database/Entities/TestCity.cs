using System.Collections.Generic;

namespace Tests.Infrastructure.Database.Entities;

public class TestCity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<TestCompany>? Companies { get; set; }
}
