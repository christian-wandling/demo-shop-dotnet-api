using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Order.Entities;

namespace DemoShop.Domain.Order.Specifications;

public sealed class OrderSpecifications : IOrderSpecifications
{
    public Specification<OrderEntity> GetById(int id, string userEmail) => new OrderByIdSpecification(id, userEmail);
}
