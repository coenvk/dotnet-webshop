using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Contracts.Events;

public sealed record OrderCreated
{
    public OrderCreated(Ulid orderId, IList<OrderLineDto> orderLines)
    {
        OrderId = orderId;
        OrderLines = orderLines;
    }

    [Required]
    public Ulid OrderId { get; set; }

    [MinLength(1)]
    public IList<OrderLineDto> OrderLines { get; set; }
}