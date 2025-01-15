using System.Linq.Expressions;

namespace DemoShop.Domain.Common.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    Expression<Func<T, bool>> ById(int id);
    IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }
    IReadOnlyCollection<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int? Take { get; }
    int? Skip { get; }
}
