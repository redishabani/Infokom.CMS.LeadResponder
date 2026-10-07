using Infokom.CMS.LeadResponder.Application;
using Infokom.CMS.LeadResponder.Domain;
using Microsoft.Extensions.Options;

namespace Infokom.CMS.LeadResponder.Infrastructure;

public sealed class BusinessContextOptions
{
    public const string SectionName = "BusinessContext";

    public string BusinessName { get; set; } = "Our Business";

    public string Description { get; set; } = "";
}

public sealed class ConfiguredBusinessContextProvider(IOptions<BusinessContextOptions> options) : IBusinessContextProvider
{
    public BusinessContext GetContext() =>
        new(options.Value.BusinessName, options.Value.Description);
}
