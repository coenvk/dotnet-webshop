using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShop.Inventory.Api.Domain;

namespace WebShop.Inventory.Api.Infrastructure;

internal sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.Property(x => x.Id)
            .HasValueGenerator<UlidValueGenerator>()
            .HasConversion(
                u => u.ToString(),
                u => Ulid.Parse(u));
    }
}