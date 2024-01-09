using System;

namespace WebShop.Order.Contracts.Events;

public sealed record OrderCompleted
{
    public Ulid OrderId { get; set; }
}