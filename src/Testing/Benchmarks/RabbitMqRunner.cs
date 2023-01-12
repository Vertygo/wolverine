using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using IntegrationTests;
using JasperFx.Core;
using Oakton.Resources;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.SqlServer;

namespace Benchmarks;

[MemoryDiagnoser]
public class RabbitMqRunner : IDisposable
{
    private readonly Driver theDriver;

    [Params("Postgresql", "None")] public string DatabaseEngine;
    [Params(1, 3, 5)] public int ListenerCount;

    [Params(1, 5, 10)] public int NumberOfThreads;

    public RabbitMqRunner()
    {
        theDriver = new Driver();
    }

    public void Dispose()
    {
        theDriver.SafeDispose();
    }

    [IterationSetup]
    public async Task BuildDatabase()
    {
        await theDriver.Start(opts =>
        {
            opts.Node.DurabilityAgentEnabled = false;

            switch (DatabaseEngine)
            {
                case "SqlServer":
                    opts.PersistMessagesWithSqlServer(Servers.SqlServerConnectionString);
                    break;

                case "Postgresql":
                    opts.PersistMessagesWithPostgresql(Servers.PostgresConnectionString);
                    break;
            }

            var queueName = RabbitTesting.NextQueueName();

            opts.UseRabbitMq().AutoProvision();

            opts.PublishAllMessages().ToRabbitQueue(queueName);
            opts.ListenToRabbitQueue(queueName)
                .MaximumParallelMessages(NumberOfThreads)
                .ListenerCount(ListenerCount);
        });

        await theDriver.Host.ResetResourceState();
    }

    [IterationCleanup]
    public async Task Teardown()
    {
        await theDriver.Teardown();
    }

    [Benchmark]
    public async Task EnqueueMultiThreaded()
    {
        var task1 = Task.Factory.StartNew(async () =>
        {
            foreach (var target in theDriver.Targets.Take(200)) await theDriver.Bus.PublishAsync(target);
        });

        var task2 = Task.Factory.StartNew(async () =>
        {
            foreach (var target in theDriver.Targets.Skip(200).Take(200))
                await theDriver.Bus.PublishAsync(target);
        });

        var task3 = Task.Factory.StartNew(async () =>
        {
            foreach (var target in theDriver.Targets.Skip(400).Take(200))
                await theDriver.Bus.PublishAsync(target);
        });

        var task4 = Task.Factory.StartNew(async () =>
        {
            foreach (var target in theDriver.Targets.Skip(600).Take(200))
                await theDriver.Bus.PublishAsync(target);
        });

        var task5 = Task.Factory.StartNew(async () =>
        {
            foreach (var target in theDriver.Targets.Skip(800)) await theDriver.Bus.PublishAsync(target);
        });


        await theDriver.WaitForAllEnvelopesToBeProcessed();
    }
}