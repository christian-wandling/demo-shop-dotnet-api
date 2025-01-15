using Asp.Versioning;
using DemoShop.Infrastructure;
using DemoShop.Application;
using DemoShop.Application.Features.Common.Constants;
using DemoShop.Application.Features.Common.Interfaces;
using DemoShop.Infrastructure.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(options =>
{
    options.Dsn = builder.Configuration["Sentry:Dsn"];
    options.Debug = builder.Environment.IsDevelopment();
    options.TracesSampleRate = 1.0;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:ClientId"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "roles",
            NameClaimType = KeycloakClaimTypes.Email
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireBuyProductsRole", policy =>
        policy.RequireClaim("realm_access_roles", "realm:buy_products"));

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo Shop API", Version = "v1" });
        c.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer" });
    });

builder.Services.AddApiVersioning(
    options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    });

builder.Services.AddProblemDetails();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();

    if (builder.Environment.IsDevelopment())
    {
        logging.AddConsole();
        logging.AddDebug();
    }

    logging.AddSentry(options => options.MinimumEventLevel = LogLevel.Error);
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services
    .AddHttpContextAccessor()
    .AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
