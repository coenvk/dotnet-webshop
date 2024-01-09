using System;

namespace WebShop.Payment.Contracts.Events;

public sealed record PaymentFailed
{
    public PaymentFailed(Ulid orderId)
    {
        OrderId = orderId;
    }

    public Ulid OrderId { get; set; }
}