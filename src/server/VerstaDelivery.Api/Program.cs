using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using VerstaDelivery.Api.Data;
using VerstaDelivery.Api.Endpoints;
using VerstaDelivery.Api.Services;
using VerstaDelivery.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", tags: ["ready"]);

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IOrderNumberGenerator, OrderNumberGenerator>();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("RunMigrations"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

    using (LogContext.PushProperty("TraceId", traceId))
    {
        await next();
    }
});

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        diagnosticContext.Set("TraceId", traceId);
    };

    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (httpContext.Request.Path.StartsWithSegments("/health")) return LogEventLevel.Debug;

        if (ex is not null) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;

        return LogEventLevel.Information;
    };
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapOrderEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();

public partial class Program;
