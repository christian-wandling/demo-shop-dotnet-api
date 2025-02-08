#region

using Microsoft.EntityFrameworkCore.Infrastructure;

#endregion

namespace DemoShop.Application.Common.Interfaces;

public interface IApplicationDbContext : IDisposable
{
    DatabaseFacade Database { get; }
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
