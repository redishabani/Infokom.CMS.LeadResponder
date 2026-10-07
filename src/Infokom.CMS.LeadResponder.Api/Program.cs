using Infokom.CMS.LeadResponder.Application;
using Infokom.CMS.LeadResponder.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLeadResponder(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ErrorHandler");
    if (exception is not null)
        logger.LogError(exception, "Unhandled exception");

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await Results.Problem(title: "An unexpected error occurred.", statusCode: 500).ExecuteAsync(context);
}));

app.MapHealthChecks("/health");

app.MapPost("/api/chat", async (ChatRequest request, IChatService chat, CancellationToken ct) =>
{
    try
    {
        return Results.Ok(await chat.SendAsync(request, ct));
    }
    catch (ChatValidationException ex)
    {
        return Results.ValidationProblem(ex.Errors.ToDictionary(e => e.Key, e => e.Value));
    }
});

app.Run();

public partial class Program;
