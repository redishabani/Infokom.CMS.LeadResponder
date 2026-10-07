using System.Collections.Concurrent;
using Infokom.CMS.LeadResponder.Application;
using Infokom.CMS.LeadResponder.Domain;

namespace Infokom.CMS.LeadResponder.Infrastructure;

public sealed class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, Conversation> _conversations = new();

    public Task<Conversation?> GetAsync(string conversationId, CancellationToken cancellationToken) =>
        Task.FromResult(_conversations.GetValueOrDefault(conversationId));

    public Task SaveAsync(Conversation conversation, CancellationToken cancellationToken)
    {
        _conversations[conversation.Id] = conversation;
        return Task.CompletedTask;
    }
}
