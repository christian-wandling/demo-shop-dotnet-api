namespace DemoShop.Domain.Common.Interfaces;

public interface IUserIdentity
{
    string Email { get; }
    string KeycloakId { get; }
    string FirstName { get; }
    string LastName { get; }
}
