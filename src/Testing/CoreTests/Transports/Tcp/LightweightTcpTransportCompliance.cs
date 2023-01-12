using System.Threading.Tasks;
using TestingSupport;
using TestingSupport.Compliance;
using Wolverine.Transports.Tcp;
using Wolverine.Util;
using Xunit;

namespace CoreTests.Transports.Tcp;

public class LightweightTcpFixture : TransportComplianceFixture, IAsyncLifetime
{
    public LightweightTcpFixture() : base($"tcp://localhost:{PortFinder.GetAvailablePort()}/incoming".ToUri())
    {
    }

    public Task InitializeAsync()
    {
        SenderIs(opts => { opts.ListenAtPort(PortFinder.GetAvailablePort()); });

        ReceiverIs(opts => { opts.ListenAtPort(OutboundAddress.Port); });
        
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}

[Collection("compliance")]
public class LightweightTcpTransportCompliance : TransportCompliance<LightweightTcpFixture>
{
}