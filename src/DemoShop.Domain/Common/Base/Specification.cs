using System.Linq.Expressions;
using DemoShop.Domain.Common.Interfaces;

namespace DemoShop.Domain.Common.Base;

public abstract class Specification<T> where T : IEntity
{
    private readonly List<Expression<Func<T, bool>>> _criteria = [];
    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<string> _includeStrings = [];

    public IReadOnlyCollection<string> IncludeStrings => _includeStrings;
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }

    protected virtual Expression<Func<T, bool>> Criteria =>
        _criteria.Count switch
        {
            0 => _ => true,
            1 => _criteria[0],
            _ => _criteria.Aggregate(CombineExpressions)
        };

    protected virtual void AddCriteria(Expression<Func<T, bool>> criteria) =>
        _criteria.Add(criteria ?? throw new ArgumentNullException(nameof(criteria)));

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        var param = Expression.Parameter(typeof(T), "x");
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(first, param),
                Expression.Invoke(second, param)
            ), param);
    }

    protected virtual void AddInclude(Expression<Func<T, object>> include) =>
        _includes.Add(include);

    protected virtual void AddOrderBy(Expression<Func<T, object>> orderBy) =>
        OrderBy = orderBy;

    protected virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDesc) =>
        OrderByDescending = orderByDesc;

    protected virtual void ApplyPaging(int skip, int take) =>
        (Skip, Take) = (skip, take);
}
