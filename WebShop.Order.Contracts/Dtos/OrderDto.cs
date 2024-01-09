using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Order.Contracts.Dtos;

public sealed record OrderDto
{
    [Required]
    public Ulid OrderId { get; set; }

    [Required]
    public string AddressLine { get; set; } = string.Empty;
    
    public OrderStatus Status { get; set; }

    [MinLength(1)]
    public IList<OrderLineDto> OrderLines = Array.Empty<OrderLineDto>();
}