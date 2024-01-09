using System;

namespace WebShop.Order.Contracts.Events;

public sealed record OrderCancelled
{
    public Ulid OrderId { get; set; }
}