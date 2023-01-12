using System.Threading.Tasks;
using IntegrationTests;
using Marten;
using TestingSupport;
using TestingSupport.Compliance;
using Wolverine.Marten;
using Wolverine.Util;
using Xunit;

namespace PersistenceTests.Marten;

[Collection("marten")]
public class DurableTcpTransportFixture : TransportComplianceFixture, IAsyncLifetime
{
    public DurableTcpTransportFixture() : base($"tcp://localhost:{PortFinder.GetAvailablePort()}/incoming".ToUri())
    {
    }

    public Task InitializeAsync()
    {
        OutboundAddress = $"tcp://localhost:{PortFinder.GetAvailablePort()}/incoming/durable".ToUri();

        SenderIs(opts =>
        {
            var receivingUri = $"tcp://localhost:{PortFinder.GetAvailablePort()}/incoming/durable".ToUri();
            opts.ListenForMessagesFrom(receivingUri);

            opts.Services.AddMarten(o =>
            {
                o.Connection(Servers.PostgresConnectionString);
                o.DatabaseSchemaName = "sender";
            }).IntegrateWithWolverine();
        });

        ReceiverIs(opts =>
        {
            opts.ListenForMessagesFrom(OutboundAddress);

            opts.Services.AddMarten(o =>
            {
                o.Connection(Servers.PostgresConnectionString);
                o.DatabaseSchemaName = "receiver";
            }).IntegrateWithWolverine();
        });
        
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}

[Collection("marten")]
public class DurableTcpTransportCompliance : TransportCompliance<DurableTcpTransportFixture>
{
}