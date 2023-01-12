using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using IntegrationTests;
using JasperFx.Core;
using Newtonsoft.Json;
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.SqlServer;

namespace Benchmarks;

[MemoryDiagnoser]
public class PersistenceRunner : IDisposable
{
    private readonly Driver theDriver;

    [Params("SqlServer", "Postgresql")] public string DatabaseEngine;
    private Envelope[] theEnvelopes;

    public PersistenceRunner()
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
            if (DatabaseEngine == "SqlServer")
            {
                opts.PersistMessagesWithSqlServer(Servers.SqlServerConnectionString);
            }
            else
            {
                opts.PersistMessagesWithPostgresql(Servers.PostgresConnectionString);
            }
        });

        theEnvelopes = theDriver.Targets.Select(x =>
        {
            var stream = new MemoryStream();
            var writer = new JsonTextWriter(new StreamWriter(stream));
            new JsonSerializer().Serialize(writer, x);
            var env = new Envelope(x);
            env.Destination = new Uri("fake://localhost:5000");
            stream.Position = 0;
            env.Data = stream.ReadAllBytes();

            env.ContentType = EnvelopeConstants.JsonContentType;
            env.MessageType = "target";


            return env;
        }).ToArray();
    }

    [IterationCleanup]
    public async Task Teardown()
    {
        await theDriver.Teardown();
    }

    [Benchmark]
    public async Task StoreIncoming()
    {
        for (var i = 0; i < 10; i++)
        {
            await theDriver.Persistence.StoreIncomingAsync(theEnvelopes.Skip(i * 100).Take(100).ToArray());
        }
    }

    [Benchmark]
    public async Task StoreOutgoing()
    {
        for (var i = 0; i < 10; i++)
        {
            await theDriver.Persistence.StoreOutgoingAsync(theEnvelopes.Skip(i * 100).Take(100).ToArray(), 5);
        }
    }

    [IterationSetup(Target = nameof(LoadIncoming))]
    public async Task LoadIncomingSetup()
    {
        BuildDatabase();
        await StoreIncoming();
    }

    [Benchmark]
    public Task LoadIncoming()
    {
        return theDriver.Persistence.Admin.AllIncomingAsync();
    }

    [IterationSetup(Target = nameof(LoadOutgoing))]
    public async Task LoadOutgoingSetup()
    {
        BuildDatabase();
        await StoreOutgoing();
    }

    [Benchmark]
    public Task LoadOutgoing()
    {
        return theDriver.Persistence.Admin.AllOutgoingAsync();
    }
}