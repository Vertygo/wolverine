using IntegrationTests;
using Marten;
using TestingSupport.Compliance;
using Wolverine.Marten;

namespace Wolverine.AmazonSqs.Tests;

public class DurableComplianceFixture : TransportComplianceFixture, IAsyncLifetime
{
    public DurableComplianceFixture() : base(new Uri("sqs://durable-receiver"), 120)
    {
    }

    public Task InitializeAsync()
    {
        SenderIs(opts =>
        {
            opts.UseAmazonSqsTransportLocally()
                .AutoProvision()
                .AutoPurgeOnStartup()
                .ConfigureListeners(x => x.UseDurableInbox())
                .ConfigureListeners(x => x.UseDurableInbox());

            opts.Services.AddMarten(store =>
            {
                store.Connection(Servers.PostgresConnectionString);
                store.DatabaseSchemaName = "sender";
            }).IntegrateWithWolverine("sender").ApplyAllDatabaseChangesOnStartup();

            opts.ListenToSqsQueue("durable-sender");
        });

        ReceiverIs(opts =>
        {
            opts.UseAmazonSqsTransportLocally()
                .AutoProvision()
                .AutoPurgeOnStartup()
                .ConfigureListeners(x => x.UseDurableInbox())
                .ConfigureListeners(x => x.UseDurableInbox());

            opts.Services.AddMarten(store =>
            {
                store.Connection(Servers.PostgresConnectionString);
                store.DatabaseSchemaName = "receiver";
            }).IntegrateWithWolverine("receiver").ApplyAllDatabaseChangesOnStartup();

            opts.ListenToSqsQueue("durable-receiver");
        });

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    [Collection("acceptance")]
    public class DurableSendingAndReceivingCompliance : TransportCompliance<DurableComplianceFixture>
    {
    }
}