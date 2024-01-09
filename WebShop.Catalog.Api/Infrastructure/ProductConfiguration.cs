using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShop.Catalog.Api.Domain;

namespace WebShop.Catalog.Api.Infrastructure;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Id)
            .HasValueGenerator<UlidValueGenerator>()
            .HasConversion(
                u => u.ToString(),
                u => Ulid.Parse(u));
    }
}