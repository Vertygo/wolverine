using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using TestingSupport;
using Wolverine.Runtime;
using Wolverine.Runtime.Routing;

namespace Wolverine.AmazonSqs.Tests.ConventionalRouting;

public abstract class ConventionalRoutingContext : IDisposable
{
    private IHost _host;

    internal IWolverineRuntime theRuntime
    {
        get
        {
            _host ??= WolverineHost.For(opts =>
                opts.UseAmazonSqsTransportLocally().UseConventionalRouting().AutoProvision().AutoPurgeOnStartup()).GetAwaiter().GetResult();

            return _host.Services.GetRequiredService<IWolverineRuntime>();
        }
    }

    public void Dispose()
    {
        _host?.Dispose();
    }

    internal async Task ConfigureConventions(Action<AmazonSqsMessageRoutingConvention> configure)
    {
        _host = await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.UseAmazonSqsTransportLocally().UseConventionalRouting(configure).AutoProvision()
                    .AutoPurgeOnStartup();
            }).StartAsync();
    }

    internal IMessageRouter RoutingFor<T>()
    {
        return theRuntime.RoutingFor(typeof(T));
    }

    internal void AssertNoRoutes<T>()
    {
        RoutingFor<T>().ShouldBeOfType<EmptyMessageRouter<T>>();
    }

    internal MessageRoute[] PublishingRoutesFor<T>()
    {
        return RoutingFor<T>().ShouldBeOfType<MessageRouter<T>>().Routes;
    }
}