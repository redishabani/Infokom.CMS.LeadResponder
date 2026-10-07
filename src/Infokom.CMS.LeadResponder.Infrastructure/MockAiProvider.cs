using Infokom.CMS.LeadResponder.Application;
using Infokom.CMS.LeadResponder.Domain;

namespace Infokom.CMS.LeadResponder.Infrastructure;

/// <summary>Local provider that needs no credentials. Replace with a real provider later.</summary>
public sealed class MockAiProvider : IAiProvider
{
    public Task<string> GenerateReplyAsync(
        BusinessContext context,
        IReadOnlyList<ChatMessage> history,
        CancellationToken cancellationToken)
    {
        var last = history.Last(m => m.Role == ChatRole.User).Content;
        return Task.FromResult($"Thanks for contacting {context.BusinessName}! You said: \"{last}\". A team member will follow up shortly.");
    }
}
