namespace Infokom.CMS.LeadResponder.Domain;

public enum ChatRole
{
    User,
    Assistant
}

public sealed record ChatMessage(ChatRole Role, string Content, DateTimeOffset Timestamp);
