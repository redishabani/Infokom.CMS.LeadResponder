using Infokom.CMS.LeadResponder.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infokom.CMS.LeadResponder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLeadResponder(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BusinessContextOptions>(configuration.GetSection(BusinessContextOptions.SectionName));
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IBusinessContextProvider, ConfiguredBusinessContextProvider>();
        services.AddSingleton<IConversationStore, InMemoryConversationStore>();
        services.AddSingleton<IAiProvider, MockAiProvider>();
        services.AddScoped<IChatService, ChatService>();
        return services;
    }
}
