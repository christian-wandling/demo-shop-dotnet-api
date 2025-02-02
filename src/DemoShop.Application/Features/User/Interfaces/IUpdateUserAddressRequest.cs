namespace DemoShop.Application.Features.User.Interfaces;

public interface IUpdateUserAddressRequest
{
    public string Street { get; set; }
    public string Apartment { get; set; }
    public string City { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string? Region { get; set; }
}
