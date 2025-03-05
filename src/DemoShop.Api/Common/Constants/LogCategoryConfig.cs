using System.Collections.ObjectModel;
using DemoShop.Api.Common.Models;

namespace DemoShop.Api.Common.Constants;

public static class LogCategoryConfig
{
    public static readonly ReadOnlyCollection<LogCategory> Categories = new(
        new List<LogCategory>
        {
            new(1000, 1999, "logs/application-.txt"),
            new(2000, 2999, "logs/authentication-.txt"),
            new(3000, 3999, "logs/api-requests-.txt"),
            new(4000, 4999, "logs/business-logic-.txt"),
            new(5000, 5999, "logs/domain-events-.txt"),
            new(6000, 6999, "logs/domain-exceptions-.txt"),
            new(7000, 7999, "logs/data-access-.txt")
        });
}
