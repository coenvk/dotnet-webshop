using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Order.Contracts.Commands;

public sealed record CancelOrder
{
    public CancelOrder(Ulid orderId)
    {
        OrderId = orderId;
    }

    [Required]
    public Ulid OrderId { get; set; }
}