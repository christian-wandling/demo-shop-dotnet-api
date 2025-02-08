#region

using DemoShop.Domain.Common.Interfaces;

#endregion

namespace DemoShop.TestUtils.Features.User.Models;

public sealed record TestUserIdentity : IUserIdentity
{
    public required string Email { get; set; }
    public required string KeycloakUserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
