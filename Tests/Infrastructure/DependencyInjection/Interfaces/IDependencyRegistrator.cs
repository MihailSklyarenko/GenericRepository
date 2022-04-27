using System;

namespace Tests.Infrastructure.DependencyInjection.Interfaces;

public interface IDependencyRegistrator
{
    IServiceProvider GetServiceProvider();
}