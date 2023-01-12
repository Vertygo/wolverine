using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using IntegrationTests;
using JasperFx.Core;
using Wolverine.Postgresql;
using Wolverine.SqlServer;

namespace Benchmarks;

[MemoryDiagnoser]
public class LocalRunner : IDisposable
{
    private readonly Driver theDriver;

    [Params("SqlServer", "Postgresql", "None")]
    public string DatabaseEngine;

    [Params(1, 5, 10)] public int NumberOfThreads;

    public LocalRunner()
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

            opts.DefaultLocalQueue
                .MaximumParallelMessages(NumberOfThreads);
        });
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