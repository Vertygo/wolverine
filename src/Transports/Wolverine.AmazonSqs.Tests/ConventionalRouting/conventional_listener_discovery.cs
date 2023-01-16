using Shouldly;
using TestMessages;
using Wolverine.AmazonSqs.Internal;
using Wolverine.Configuration;
using Wolverine.Util;

namespace Wolverine.AmazonSqs.Tests.ConventionalRouting;

public class conventional_listener_discovery : ConventionalRoutingContext
{
    [Fact]
    public async Task disable_sender_with_lambda()
    {
        await ConfigureConventions(c => c.QueueNameForSender(t =>
        {
            if (t == typeof(PublishedMessage))
            {
                return null; // should not be routed
            }

            return t.ToMessageTypeName().Replace(".", "-");
        }));

        AssertNoRoutes<PublishedMessage>();
    }

    [Fact]
    public async Task exclude_types()
    {
        await ConfigureConventions(c => { c.ExcludeTypes(t => t == typeof(PublishedMessage)); });

        AssertNoRoutes<PublishedMessage>();

        var uri = "sqs://published.message".ToUri();
        var endpoint = theRuntime.Endpoints.EndpointFor(uri);
        endpoint.ShouldBeNull();

        theRuntime.Endpoints.ActiveListeners().Any(x => x.Uri == uri)
            .ShouldBeFalse();
    }

    [Fact]
    public async Task include_types()
    {
        await ConfigureConventions(c => { c.IncludeTypes(t => t == typeof(PublishedMessage)); });

        AssertNoRoutes<Message1>();

        PublishingRoutesFor<PublishedMessage>().Any().ShouldBeTrue();

        var uri = "sqs://Message1".ToUri();
        var endpoint = theRuntime.Endpoints.EndpointFor(uri);
        endpoint.ShouldBeNull();

        theRuntime.Endpoints.ActiveListeners().Any(x => x.Uri == uri)
            .ShouldBeFalse();
    }

    [Fact]
    public async Task configure_sender_overrides()
    {
        await ConfigureConventions(c => c.ConfigureSending((c, _) => c.AddOutgoingRule(new FakeEnvelopeRule())));

        var route = PublishingRoutesFor<PublishedMessage>().Single().Sender.Endpoint
            .ShouldBeOfType<AmazonSqsQueue>();

        route.OutgoingRules.Single().ShouldBeOfType<FakeEnvelopeRule>();
    }

    [Fact]
    public async Task disable_listener_by_lambda()
    {
        await ConfigureConventions(c => c.QueueNameForListener(t =>
        {
            if (t == typeof(RoutedMessage))
            {
                return null; // should not be routed
            }

            return t.ToMessageTypeName().Replace(".", "-");
        }));

        var uri = "sqs://routed".ToUri();
        var endpoint = theRuntime.Endpoints.EndpointFor(uri);
        endpoint.ShouldBeNull();

        theRuntime.Endpoints.ActiveListeners().Any(x => x.Uri == uri)
            .ShouldBeFalse();
    }

    [Fact]
    public async Task configure_listener()
    {
        await ConfigureConventions(c => c.ConfigureListeners((x, _) => { x.UseDurableInbox(); }));

        var endpoint = theRuntime.Endpoints.EndpointFor("sqs://routed".ToUri())
            .ShouldBeOfType<AmazonSqsQueue>();

        endpoint.Mode.ShouldBe(EndpointMode.Durable);
    }

    public class FakeEnvelopeRule : IEnvelopeRule
    {
        public void Modify(Envelope envelope)
        {
            throw new NotImplementedException();
        }
    }
}