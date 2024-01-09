using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace WebShop.Catalog.Api.Infrastructure;

public sealed class UlidValueGenerator : ValueGenerator<Ulid>
{
    public override Ulid Next(EntityEntry entry)
    {
        return Ulid.NewUlid();
    }

    public override bool GeneratesTemporaryValues { get; } = false;
}