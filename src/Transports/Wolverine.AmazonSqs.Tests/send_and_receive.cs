using JasperFx.Core;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Wolverine.Tracking;

namespace Wolverine.AmazonSqs.Tests;

public class send_and_receive : IAsyncLifetime
{
    private IHost _host;

    public async Task InitializeAsync()
    {
        _host = await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                opts.UseAmazonSqsTransportLocally()
                    .AutoProvision().AutoPurgeOnStartup();

                opts.ListenToSqsQueue("send_and_receive");

                opts.PublishAllMessages().ToSqsQueue("send_and_receive");
            }).StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
    }

    [Fact]
    public async Task send_and_receive_a_single_message()
    {
        var message = new SqsMessage("Josh Allen");

        var session = await _host.TrackActivity()
            .IncludeExternalTransports()
            .Timeout(5.Minutes())
            .SendMessageAndWaitAsync(message);

        session.Received.SingleMessage<SqsMessage>()
            .Name.ShouldBe(message.Name);
    }
}

public record SqsMessage(string Name);

public static class SqsMessageHandler
{
    public static void Handle(SqsMessage message)
    {
        // nothing
    }
}