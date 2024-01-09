using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Inventory.Contracts.Dtos;

public sealed record StockDto
{
    [Required]
    public int Quantity { get; set; }

    [Required]
    public Ulid ProductId { get; set; }
}