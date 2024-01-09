using System;
using System.ComponentModel.DataAnnotations.Schema;
using Toolkit.Domain.Entities;

namespace WebShop.Inventory.Api.Domain;

[Table("stock")]
public sealed class Stock : AggregateRoot<Ulid>
{
    internal Stock() : base(Ulid.NewUlid())
    {
    }

    public Stock(Ulid productId) : base(productId)
    {
    }

    public int Quantity { get; set; }
    public Ulid ProductId => Id;

    public void Restock(int quantity)
    {
        Quantity += quantity;
    }
}