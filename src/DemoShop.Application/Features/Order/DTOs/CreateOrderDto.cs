using System.ComponentModel.DataAnnotations;
using DemoShop.Application.Features.ShoppingSession.DTOs;

namespace DemoShop.Application.Features.Order.DTOs;

public class CreateOrderDto
{
    public required int ShoppingSessionId { get; set; }
    [Range(1, int.MaxValue)] public required int UserId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Cart must contain at least one item")]
    public ICollection<CartItemResponse> CartItems { get; } = new List<CartItemResponse>();
}
