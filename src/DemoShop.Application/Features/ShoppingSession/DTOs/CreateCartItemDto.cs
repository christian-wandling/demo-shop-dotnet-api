using System.ComponentModel.DataAnnotations;

namespace DemoShop.Application.Features.ShoppingSession.DTOs;

public class CreateCartItemDto
{
    [Range(1, int.MaxValue)] public required int ProductId { get; set; }
}
