using Infokom.CMS.LeadResponder.Domain;

namespace Infokom.CMS.LeadResponder.Application;

public interface IAiProvider
{
    Task<string> GenerateReplyAsync(
        BusinessContext context,
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken);
}

public interface IConversationStore
{
    Task<Conversation?> GetAsync(string conversationId, CancellationToken cancellationToken);

    Task SaveAsync(Conversation conversation, CancellationToken cancellationToken);
}

public interface IBusinessContextProvider
{
    BusinessContext GetContext();
}
