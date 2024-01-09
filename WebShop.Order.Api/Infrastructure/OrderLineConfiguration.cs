using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebShop.Order.Api.Infrastructure;

internal sealed class OrderLineConfiguration : IEntityTypeConfiguration<Domain.OrderLine>
{
    public void Configure(EntityTypeBuilder<Domain.OrderLine> builder)
    {
        builder.Property(x => x.Id)
            .HasValueGenerator<UlidValueGenerator>()
            .HasConversion(
                u => u.ToString(),
                u => Ulid.Parse(u));

        builder.Property(x => x.ProductId)
            .HasValueGenerator<UlidValueGenerator>()
            .HasConversion(
                u => u.ToString(),
                u => Ulid.Parse(u));
    }
}