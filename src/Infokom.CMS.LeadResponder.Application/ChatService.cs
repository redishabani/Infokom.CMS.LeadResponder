using Infokom.CMS.LeadResponder.Domain;

namespace Infokom.CMS.LeadResponder.Application;

public sealed record ChatRequest(string? ConversationId, string? Message);

public sealed record ChatResponse(string ConversationId, string Reply);

public sealed class ChatValidationException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("The chat request is invalid.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}

public interface IChatService
{
    Task<ChatResponse> SendAsync(ChatRequest request, CancellationToken cancellationToken);
}

public sealed class ChatService(
    IAiProvider aiProvider,
    IConversationStore store,
    IBusinessContextProvider contextProvider,
    TimeProvider timeProvider) : IChatService
{
    public const int MaxMessageLength = 2000;
    public const int MaxConversationIdLength = 64;

    public async Task<ChatResponse> SendAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var id = string.IsNullOrWhiteSpace(request.ConversationId)
            ? Guid.NewGuid().ToString("N")
            : request.ConversationId.Trim();

        var conversation = await store.GetAsync(id, cancellationToken) ?? new Conversation(id);
        conversation.Add(ChatRole.User, request.Message!.Trim(), timeProvider.GetUtcNow());

        var reply = await aiProvider.GenerateReplyAsync(
            contextProvider.GetContext(), conversation.Messages, cancellationToken);

        conversation.Add(ChatRole.Assistant, reply, timeProvider.GetUtcNow());
        await store.SaveAsync(conversation, cancellationToken);

        return new ChatResponse(id, reply);
    }

    private static void Validate(ChatRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Message))
            errors["message"] = ["Message is required."];
        else if (request.Message.Length > MaxMessageLength)
            errors["message"] = [$"Message must be at most {MaxMessageLength} characters."];

        if (request.ConversationId is { Length: > MaxConversationIdLength })
            errors["conversationId"] = [$"ConversationId must be at most {MaxConversationIdLength} characters."];

        if (errors.Count > 0)
            throw new ChatValidationException(errors);
    }
}
