using System.ComponentModel.DataAnnotations;

namespace DemoShop.Application.Features.Session.DTOs;

public class CreateCartItemDto
{
    [Range(1, int.MaxValue)] public required int ProductId { get; set; }
}
