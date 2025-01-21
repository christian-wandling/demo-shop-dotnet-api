using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DemoShop.Infrastructure.Common.Persistence;

public class ModifiedAtPropertyConvention : IModelFinalizingConvention
{
    private const string ModifiedAtPropertyName = "ModifiedAt";
    private const string UpdatedAtColumn = "updatedAt";

    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        Guard.Against.Null(modelBuilder, nameof(modelBuilder));

        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            var property = entityType.FindProperty(ModifiedAtPropertyName);
            property?.SetColumnName(UpdatedAtColumn);
        }
    }
}
