#region

using DemoShop.Infrastructure.Common.Persistence;
using DemoShop.TestUtils.Common.Base;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

#endregion

namespace DemoShop.Infrastructure.Tests.Common.Base;

[Trait("Category", "Unit")]
[Trait("Layer", "Infrastructure")]
public abstract class RepositoryTest : Test
{
    protected readonly ApplicationDbContext Context;

    protected RepositoryTest(ITestOutputHelper? output = null) : base(output)
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(dbOptions);
    }

    protected async Task<T> AddTestDataAsync<T>(T entity) where T : class
    {
        var entry = Context.Set<T>().Add(entity);
        await Context.SaveChangesAsync();

        return entry.Entity;
    }

    protected async Task<T> UpdateTestDataAsync<T>(T entity) where T : class
    {
        var entry = Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();

        return entry.Entity;
    }

    protected async Task AddTestDataRangeAsync<T>(IEnumerable<T> entities) where T : class
    {
        Context.Set<T>().AddRange(entities);
        await Context.SaveChangesAsync();
    }
}
