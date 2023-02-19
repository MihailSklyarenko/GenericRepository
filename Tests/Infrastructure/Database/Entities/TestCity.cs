using GenericRepository.Entities;
using System.Collections.Generic;

namespace Tests.Infrastructure.Database.Entities;

public class TestCity : BaseEntity<int>
{
    public string Name { get; set; }

    public ICollection<TestCompany>? Companies { get; set; }
}
