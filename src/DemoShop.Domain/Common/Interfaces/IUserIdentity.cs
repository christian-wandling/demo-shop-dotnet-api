namespace DemoShop.Domain.Common.Interfaces;

public interface IUserIdentity
{
    string Email { get; }
    Guid KeycloakId { get; }
    string FirstName { get; }
    string LastName { get; }
}
