using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Services;
using Wolverine.Runtime;
using Microsoft.VisualStudio.Threading;

namespace Wolverine.Marten;

internal class FlushOutgoingMessagesOnCommit : DocumentSessionListenerBase
{
    private readonly MessageContext _context;

    public FlushOutgoingMessagesOnCommit(MessageContext context)
    {
        _context = context;
    }

    public override void AfterCommit(IDocumentSession session, IChangeSet commit)
    {
        JoinableTaskFactory joinableTaskFactory = new JoinableTaskFactory(new JoinableTaskContext());
        joinableTaskFactory.Run(async delegate { await _context.FlushOutgoingMessagesAsync(); });
    }

    public override Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        return _context.FlushOutgoingMessagesAsync();
    }
}