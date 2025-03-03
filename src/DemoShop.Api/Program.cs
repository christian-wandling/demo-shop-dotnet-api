#region

using DemoShop.Api.Common.Configurations;
using DemoShop.Domain.Common.Logging;
using Serilog;

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();
LoggingConfiguration.ConfigureLogging(builder);

builder.ConfigureServices();

var app = builder.Build();
app.ConfigureMiddleware();

app.Run();

public partial class Program
{
}
