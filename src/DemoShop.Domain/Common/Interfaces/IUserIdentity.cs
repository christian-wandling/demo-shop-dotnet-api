namespace DemoShop.Domain.Common.Interfaces;

public interface IUserIdentity
{
    string Email { get; }
    string KeycloakUserId { get; }
    string FirstName { get; }
    string LastName { get; }
}
