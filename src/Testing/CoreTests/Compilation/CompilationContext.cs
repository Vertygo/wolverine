using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TestingSupport;
using Wolverine.Runtime;
using Wolverine.Runtime.Handlers;
using Xunit;

namespace CoreTests.Compilation;

public abstract class CompilationContext : IDisposable
{
    public readonly WolverineOptions theOptions = new();

    private IHost _host;


    protected Envelope theEnvelope;

    public CompilationContext()
    {
        theOptions.Handlers.DisableConventionalDiscovery();
    }

    public void Dispose()
    {
        _host?.Dispose();
    }

    protected async Task AllHandlersCompileSuccessfully()
    {
        using var host = await WolverineHost.For(theOptions);
        host.Get<HandlerGraph>().Chains.Length.ShouldBeGreaterThan(0);
    }

    public async Task<MessageHandler> HandlerFor<TMessage>()
    {
        if (_host == null)
        {
            _host = await WolverineHost.For(theOptions);
        }


        return _host.Get<HandlerGraph>().HandlerFor(typeof(TMessage));
    }

    public async Task<IMessageContext> Execute<TMessage>(TMessage message)
    {
        var handler = await HandlerFor<TMessage>();
        theEnvelope = new Envelope(message);
        var context = new MessageContext(_host.Get<IWolverineRuntime>());
        context.ReadEnvelope(theEnvelope, InvocationCallback.Instance);

        await handler.HandleAsync(context, default);

        return context;
    }

    [Fact]
    public async Task can_compile_all()
    {
        await AllHandlersCompileSuccessfully();
    }
}