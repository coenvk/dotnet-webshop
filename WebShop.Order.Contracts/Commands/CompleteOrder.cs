using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Order.Contracts.Commands;

public sealed record CompleteOrder
{
    public CompleteOrder(Ulid orderId)
    {
        OrderId = orderId;
    }

    [Required]
    public Ulid OrderId { get; set; }
}