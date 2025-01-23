using DemoShop.Domain.Common.Base;

namespace DemoShop.Domain.Order.Specifications;

public interface IOrderSpecifications
{
    Specification<Entities.OrderEntity> GetById(int id, string userEmail);
}
