#region

using System.Text.Json.Serialization;

#endregion

namespace DemoShop.Domain.Order.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Created,
    Completed
}
