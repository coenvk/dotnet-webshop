using System;

namespace WebShop.Catalog.Contracts.Events;

public sealed record ProductCreated
{
    public ProductCreated(Ulid productId)
    {
        ProductId = productId;
    }

    public Ulid ProductId { get; set; }
}