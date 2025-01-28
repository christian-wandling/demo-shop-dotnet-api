using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Order.Entities;

namespace DemoShop.Domain.Order.Specifications;

public sealed class OrderByIdSpecification : Specification<OrderEntity>
{
    public OrderByIdSpecification(int id, string userEmail)
    {
        AddCriteria(o => o.Id == id);
        AddCriteria(o => o.User!.Email.Value == userEmail); // Security check using trusted token claim

        AddInclude(o => o.OrderItems);
    }
}
