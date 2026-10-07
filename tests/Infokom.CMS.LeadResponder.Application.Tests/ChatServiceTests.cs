using Infokom.CMS.LeadResponder.Application;
using Infokom.CMS.LeadResponder.Domain;
using Infokom.CMS.LeadResponder.Infrastructure;
using Microsoft.Extensions.Options;

namespace Infokom.CMS.LeadResponder.Application.Tests;

public class ChatServiceTests
{
    private readonly InMemoryConversationStore _store = new();

    private ChatService CreateService() => new(
        new MockAiProvider(),
        _store,
        new ConfiguredBusinessContextProvider(Options.Create(new BusinessContextOptions { BusinessName = "Acme" })),
        TimeProvider.System);

    [Fact]
    public async Task SendAsync_NewConversation_ReturnsReplyAndStoresBothMessages()
    {
        var response = await CreateService().SendAsync(new ChatRequest(null, "Hello"), default);

        Assert.False(string.IsNullOrEmpty(response.ConversationId));
        Assert.Contains("Acme", response.Reply);
        var saved = await _store.GetAsync(response.ConversationId, default);
        Assert.Equal([ChatRole.User, ChatRole.Assistant], saved!.Messages.Select(m => m.Role));
    }

    [Fact]
    public async Task SendAsync_ExistingConversation_AppendsToHistory()
    {
        var service = CreateService();
        var first = await service.SendAsync(new ChatRequest(null, "Hi"), default);
        await service.SendAsync(new ChatRequest(first.ConversationId, "More"), default);

        var saved = await _store.GetAsync(first.ConversationId, default);
        Assert.Equal(4, saved!.Messages.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendAsync_EmptyMessage_Throws(string? message)
    {
        var ex = await Assert.ThrowsAsync<ChatValidationException>(
            () => CreateService().SendAsync(new ChatRequest(null, message), default));
        Assert.Contains("message", ex.Errors.Keys);
    }

    [Fact]
    public async Task SendAsync_TooLongMessage_Throws()
    {
        var message = new string('a', ChatService.MaxMessageLength + 1);
        await Assert.ThrowsAsync<ChatValidationException>(
            () => CreateService().SendAsync(new ChatRequest(null, message), default));
    }
}
