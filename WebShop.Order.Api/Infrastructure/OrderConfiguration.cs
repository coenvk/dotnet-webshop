using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebShop.Order.Api.Infrastructure;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Domain.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Order> builder)
    {
        builder.Property(x => x.Id)
            .HasValueGenerator<UlidValueGenerator>()
            .HasConversion(
                u => u.ToString(),
                u => Ulid.Parse(u));
    }
}