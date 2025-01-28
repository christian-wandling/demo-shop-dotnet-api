using System.ComponentModel.DataAnnotations;

namespace DemoShop.Application.Features.ShoppingSession.DTOs;

public class UpdateCartItemDto
{
    [Range(1, int.MaxValue)] public required int Quantity { get; set; }
}
