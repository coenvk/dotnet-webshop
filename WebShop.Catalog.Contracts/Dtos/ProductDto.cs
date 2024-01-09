using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Catalog.Contracts.Dtos;

public sealed record ProductDto
{
    [Required]
    public Ulid ProductId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public int UnitPrice { get; set; }
}