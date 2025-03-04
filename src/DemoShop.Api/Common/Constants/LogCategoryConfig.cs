namespace DemoShop.Api.Common.Constants;

public static class LogCategoryConfig
{
    public static readonly Dictionary<int, (int maxId, string filePath)> Categories = new()
    {
        { 1000, (1999, "logs/application-.txt") },
        { 2000, (2999, "logs/authentication-.txt") },
        { 3000, (3999, "logs/api-requests-.txt") },
        { 4000, (4999, "logs/business-logic-.txt") },
        { 5000, (5999, "logs/domain-events-.txt") },
        { 6000, (6999, "logs/domain-exceptions-.txt") },
        { 7000, (7999, "logs/data-access-.txt") }
    };
}
