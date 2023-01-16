using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using TestingSupport;
using Wolverine.RabbitMQ.Internal;
using Wolverine.Runtime;
using Wolverine.Runtime.Routing;

namespace Wolverine.RabbitMQ.Tests.ConventionalRouting;

public abstract class ConventionalRoutingContext : IDisposable
{
    private IHost _host;

    internal IWolverineRuntime theRuntime
    {
        get
        {
            if (_host == null)
            {
                _host = WolverineHost.For(opts =>
                    opts.UseRabbitMq().UseConventionalRouting().AutoProvision().AutoPurgeOnStartup()).GetAwaiter().GetResult();
            }

            return _host.Services.GetRequiredService<IWolverineRuntime>();
        }
    }

    internal RabbitMqTransport theTransport
    {
        get
        {
            if (_host == null)
            {
                _host = WolverineHost.For(opts => opts.UseRabbitMq().UseConventionalRouting()).GetAwaiter().GetResult();
            }

            var options = _host.Services.GetRequiredService<IWolverineRuntime>().Options;

            return options.RabbitMqTransport();
        }
    }

    public void Dispose()
    {
        _host?.Dispose();
    }

    internal async Task ConfigureConventions(Action<RabbitMqMessageRoutingConvention> configure)
    {
        _host = await WolverineHost.For(opts =>
        {
            opts.UseRabbitMq().UseConventionalRouting(configure).AutoProvision().AutoPurgeOnStartup();
        });
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