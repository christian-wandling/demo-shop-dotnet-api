using Ardalis.GuardClauses;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DemoShop.Infrastructure.Common.Persistence;

public class LowerCasePropertiesConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        Guard.Against.Null(modelBuilder, nameof(modelBuilder));

        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
#pragma warning disable CA1308
                property.SetColumnName(property.Name.Camelize());
#pragma warning restore CA1308
            }
        }
    }
}
