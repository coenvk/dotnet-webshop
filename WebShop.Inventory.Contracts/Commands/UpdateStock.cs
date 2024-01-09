using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Inventory.Contracts.Commands;

public sealed record UpdateStock
{
    [Required]
    public Ulid OrderId { get; set; }

    [MinLength(1)]
    public IList<OrderLineDto> OrderLines { get; set; }

    public UpdateStock(Ulid orderId, IList<OrderLineDto> orderLines)
    {
        OrderId = orderId;
        OrderLines = orderLines;
    }
}