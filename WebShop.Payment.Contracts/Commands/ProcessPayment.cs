using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Payment.Contracts.Commands;

public sealed record ProcessPayment
{
    public ProcessPayment(Ulid orderId)
    {
        OrderId = orderId;
    }

    [Required]
    public Ulid OrderId { get; set; }
}