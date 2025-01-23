using System.Text.Json.Serialization;

namespace DemoShop.Domain.Order.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Created,
    Completed
}
