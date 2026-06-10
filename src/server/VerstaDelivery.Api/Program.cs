using Microsoft.EntityFrameworkCore;
using Serilog;
using VerstaDelivery.Api.Data;
using VerstaDelivery.Api.Endpoints;
using VerstaDelivery.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<IOrderNumberGenerator, OrderNumberGenerator>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

app.MapOrderEndpoints();

app.Run();
