using System;

namespace WebShop.Payment.Contracts.Events;

public sealed record PaymentCompleted
{
    public PaymentCompleted(Ulid orderId)
    {
        OrderId = orderId;
    }

    public Ulid OrderId { get; set; }
}