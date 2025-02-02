using System.Globalization;
using Asp.Versioning;
using DemoShop.Infrastructure;
using DemoShop.Application;
using Keycloak.AuthServices.Authentication;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using ServiceRegistration = DemoShop.Application.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}",
        formatProvider: CultureInfo.InvariantCulture
    )
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
        .WriteTo.File("logs/errors-.txt",
            formatProvider: CultureInfo.InvariantCulture,
            rollingInterval: RollingInterval.Day))
    .WriteTo.Sentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry:Dsn"];
        o.Debug = false;
        o.AutoSessionTracking = true;
        o.IsGlobalModeEnabled = false;
        o.TracesSampleRate = builder.Environment.IsProduction() ? 0.5 : 1.0;
        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
        o.MinimumEventLevel = LogEventLevel.Warning;
    })
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireBuyProductsRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "realm_access" &&
                c.Value.Contains("buy_products", StringComparison.OrdinalIgnoreCase)
            )
        )
    );

builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly); });

builder.Services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins(
                        builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ??
                        ["http://localhost:4200"])
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    });
builder.Services.AddApiVersioning(
        options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
    .AddMvc()
    .AddApiExplorer(
        options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo Shop API", Version = "v1" });
        c.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer" });
    });

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
