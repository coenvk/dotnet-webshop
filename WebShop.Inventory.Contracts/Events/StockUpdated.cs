using System;

namespace WebShop.Inventory.Contracts.Events;

public sealed record StockUpdated
{
    public StockUpdated(Ulid orderId)
    {
        OrderId = orderId;
    }

    public Ulid OrderId { get; set; }
}