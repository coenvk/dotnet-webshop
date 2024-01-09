using System;
using System.ComponentModel.DataAnnotations.Schema;
using Toolkit.Domain.Entities;

namespace WebShop.Catalog.Api.Domain;

[Table("product")]
public sealed class Product : AggregateRoot<Ulid>
{
    public Product() : base(Ulid.NewUlid())
    {
    }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UnitPrice { get; set; }
}