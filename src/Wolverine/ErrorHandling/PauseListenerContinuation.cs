using System;
using System.Threading;
using System.Threading.Tasks;
using Wolverine.Runtime;

namespace Wolverine.ErrorHandling;

internal class PauseListenerContinuation : IContinuation, IContinuationSource
{
    public PauseListenerContinuation(TimeSpan pauseTime)
    {
        PauseTime = pauseTime;
    }

    public TimeSpan PauseTime { get; }

    public async ValueTask ExecuteAsync(IEnvelopeLifecycle lifecycle, IWolverineRuntime runtime, DateTimeOffset now)
    {
        var agent = runtime.Endpoints.FindListeningAgent(lifecycle.Envelope!.Listener!.Address);

        if (agent != null)
        {
            await Task.Factory.StartNew(() =>
                agent.PauseAsync(PauseTime), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }
    }

    public string Description => "Pause all message processing on this listener for " + PauseTime;

    public IContinuation Build(Exception ex, Envelope envelope)
    {
        return this;
    }
}