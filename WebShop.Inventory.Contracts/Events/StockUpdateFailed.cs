using System;

namespace WebShop.Inventory.Contracts.Events;

public sealed record StockUpdateFailed
{
    public StockUpdateFailed(Ulid orderId)
    {
        OrderId = orderId;
    }

    public Ulid OrderId { get; set; }
}