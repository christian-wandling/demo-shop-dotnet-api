using DemoShop.Domain.Common.Base;
using DemoShop.Domain.Order.Entities;

namespace DemoShop.Domain.Order.Specifications;

public interface IOrderSpecifications
{
    Specification<OrderEntity> GetById(int id, string userEmail);
}
