using System.Linq.Expressions;

namespace DemoShop.Domain.Common.Interfaces;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }
    ICollection<Expression<Func<T, object>>> Includes { get; }
    ICollection<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
}

