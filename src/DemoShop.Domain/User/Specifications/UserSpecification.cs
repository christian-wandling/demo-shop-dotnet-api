using DemoShop.Domain.Common.Base;
using DemoShop.Domain.User.Entities;

namespace DemoShop.Domain.User.Specifications;

public sealed class UserByIdSpecification : Specification<UserEntity>
{
    public UserByIdSpecification(int id)
    {
        AddCriteria(user => user.Id == id);
        AddOptionalInclude(user => user.Address);
    }
}
