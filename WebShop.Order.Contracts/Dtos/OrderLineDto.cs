using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Order.Contracts.Dtos;

public sealed record OrderLineDto
{
    [Required]
    public Ulid ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}