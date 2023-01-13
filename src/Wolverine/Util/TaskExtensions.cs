﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wolverine.Util;

[Obsolete("Moved to Core")]
public static class TaskExtensions
{
    public static Task TimeoutAfterAsync(this Task task, int millisecondsTimeout)
    {
        var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        return task.ContinueWith(_ => true, scheduler).TimeoutAfterAsync(millisecondsTimeout);
    }


    // All of this was taken from https://blogs.msdn.microsoft.com/pfxteam/2011/11/10/crafting-a-task-timeoutafter-method/
    public static async Task<T> TimeoutAfterAsync<T>(this Task<T> task, int millisecondsTimeout)
    {
        // Short-circuit #1: infinite timeout or task already completed
        if (task.IsCompleted || millisecondsTimeout == Timeout.Infinite)
        {
            return await task;
        }

        // tcs.Task will be returned as a proxy to the caller
        var tcs =
            new TaskCompletionSource<T>();

        // Short-circuit #2: zero timeout
        if (millisecondsTimeout == 0)
        {
            // We've already timed out.
            tcs.SetException(new TimeoutException());
            return await tcs.Task;
        }

        // Set up a timer to complete after the specified timeout period
        var timer = new Timer(state =>
        {
            // Recover your state information
            var myTcs = (TaskCompletionSource<T>)state!;

            // Fault our proxy with a TimeoutException
            myTcs.TrySetException(new TimeoutException());
        }, tcs, millisecondsTimeout, Timeout.Infinite);

        // Wire up the logic for what happens when source task completes
         await task.ContinueWith(async (antecedent, state) =>
            {
                // Recover our state data
                var tuple =
                    (Tuple<Timer, TaskCompletionSource<T>>)state!;

                // Cancel the Timer
                tuple.Item1.Dispose();

                // Marshal results to proxy
                await MarshalTaskResults(antecedent, tuple.Item2);
            },
            Tuple.Create(timer, tcs),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

        return await tcs.Task;
    }

    internal static async Task MarshalTaskResults<TResult>(
        Task source, TaskCompletionSource<TResult> proxy)
    {
        switch (source.Status)
        {
            case TaskStatus.Faulted:
                if (source.Exception != null)
                {
                    proxy.TrySetException(source.Exception);
                }

                break;
            case TaskStatus.Canceled:
                proxy.TrySetCanceled();
                break;
            case TaskStatus.RanToCompletion:
                var castedSource = source as Task<TResult>;
                proxy.TrySetResult(
                    (castedSource == null
                        ? default
                        : // source is a Task
                        await castedSource)!); // source is a Task<TResult>
                break;
        }
    }
}