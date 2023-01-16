using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TestingSupport;
using TestingSupport.Compliance;
using TestMessages;
using Wolverine.Runtime.Handlers;
using Xunit;

namespace CoreTests;

public class DefaultApp : IDisposable
{
    public DefaultApp()
    {
        Host = WolverineHost.For(x =>
        {
            x.Handlers.IncludeType<MessageConsumer>();
            x.Handlers.IncludeType<InvokedMessageHandler>();
        }).GetAwaiter().GetResult();
    }

    public IHost Host { get; private set; }

    public void Dispose()
    {
        Host?.Dispose();
        Host = null;
    }

    public async Task RecycleIfNecessary()
    {
        if (Host == null)
        {
            Host = await WolverineHost.Basic();
        }
    }

    public HandlerChain ChainFor<T>()
    {
        return Host.Get<HandlerGraph>().ChainFor<T>();
    }
}

public class IntegrationContext : IDisposable, IClassFixture<DefaultApp>
{
    private readonly DefaultApp _default;

    public IntegrationContext(DefaultApp @default)
    {
        _default = @default;
        _default.RecycleIfNecessary().GetAwaiter().GetResult();

        Host = _default.Host;
    }

    public IHost Host { get; private set; }

    public IMessageContext Publisher => Host.Get<IMessageContext>();
    public IMessageBus Bus => Host.Get<IMessageBus>();

    public HandlerGraph Handlers => Host.Get<HandlerGraph>();

    public virtual void Dispose()
    {
        _default.Dispose();
    }


    protected async Task with(Action<WolverineOptions> configuration)
    {
        Host = await WolverineHost.For(opts =>
        {
            configuration(opts);
            opts.Services.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });
        });
    }

    protected HandlerChain chainFor<T>()
    {
        return Handlers.ChainFor<T>();
    }
}