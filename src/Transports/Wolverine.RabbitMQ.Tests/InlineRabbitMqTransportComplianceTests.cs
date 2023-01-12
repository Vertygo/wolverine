using System.Threading.Tasks;
using TestingSupport.Compliance;
using Wolverine.Util;
using Xunit;

namespace Wolverine.RabbitMQ.Tests;

public class InlineRabbitMqTransportFixture : TransportComplianceFixture, IAsyncLifetime
{
    public InlineRabbitMqTransportFixture() : base($"rabbitmq://queue/{RabbitTesting.NextQueueName()}".ToUri())
    {
    }

    public Task InitializeAsync()
    {
        var queueName = RabbitTesting.NextQueueName();
        OutboundAddress = $"rabbitmq://queue/{queueName}".ToUri();

        SenderIs(opts =>
        {
            var listener = RabbitTesting.NextQueueName();

            opts
                .ListenToRabbitQueue(listener)
                .ProcessInline();

            opts.UseRabbitMq().AutoProvision().AutoPurgeOnStartup();
        });

        ReceiverIs(opts =>
        {
            opts.UseRabbitMq().AutoProvision().AutoPurgeOnStartup();

            opts.ListenToRabbitQueue(queueName).ProcessInline();
        });
        
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}

[Collection("acceptance")]
public class InlineRabbitMqTransportComplianceTests : TransportCompliance<InlineRabbitMqTransportFixture>
{
}