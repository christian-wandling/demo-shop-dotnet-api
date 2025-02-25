#region

using DemoShop.Api.Common.Configurations;

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();
LoggingConfiguration.ConfigureLogging(builder);
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureMiddleware();
app.Run();
