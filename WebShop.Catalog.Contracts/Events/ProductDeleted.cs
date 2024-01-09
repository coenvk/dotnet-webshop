using System;

namespace WebShop.Catalog.Contracts.Events;

public sealed record ProductDeleted
{
    public ProductDeleted(Ulid productId)
    {
        ProductId = productId;
    }

    public Ulid ProductId { get; set; }
}