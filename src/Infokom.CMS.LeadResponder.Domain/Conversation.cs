namespace Infokom.CMS.LeadResponder.Domain;

public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = new();

    public Conversation(string id) => Id = id;

    public string Id { get; }

    public IReadOnlyList<ChatMessage> Messages => _messages;

    public void Add(ChatRole role, string content, DateTimeOffset timestamp) =>
        _messages.Add(new ChatMessage(role, content, timestamp));
}
