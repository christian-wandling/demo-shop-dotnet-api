using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.Order.Specifications;

public sealed class OrderSpecifications : IOrderSpecifications
{
    public Specification<Entities.OrderEntity> GetById(int id, string userEmail) => new OrderByIdSpecification(id, userEmail);
}
