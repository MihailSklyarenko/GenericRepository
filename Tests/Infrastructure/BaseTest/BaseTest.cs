using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tests.Infrastructure.DependencyInjection.Interfaces;
using Xunit;

namespace Tests.Infrastructure.BaseTest;

public abstract class BaseTest<TDependencyRegistrator> : IAsyncLifetime, IDisposable
    where TDependencyRegistrator : IDependencyRegistrator, new()
{
    private static readonly Lazy<IServiceProvider> _serviceProvider 
        = new Lazy<IServiceProvider>(() => new TDependencyRegistrator().GetServiceProvider(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly IServiceScope _serviceScope;

    protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

    protected BaseTest()
    {
        _serviceScope = _serviceProvider.Value.CreateScope();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual Task DisposeAsync() => Task.CompletedTask;

    public void Dispose() => _serviceScope?.Dispose();
}