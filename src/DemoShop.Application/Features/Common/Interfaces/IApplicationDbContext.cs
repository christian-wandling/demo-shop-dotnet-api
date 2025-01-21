namespace DemoShop.Application.Features.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
