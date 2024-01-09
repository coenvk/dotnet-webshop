using System;
using System.ComponentModel.DataAnnotations.Schema;
using Toolkit.Domain.Entities;

namespace WebShop.Order.Api.Domain;

[Table("order_line")]
public sealed class OrderLine : Entity<Ulid>
{
    internal OrderLine() : base(Ulid.NewUlid())
    {
    }

    public OrderLine(Order order) : base(Ulid.NewUlid())
    {
        Order = order;
    }

    public Ulid ProductId { get; set; }
    public int Quantity { get; set; }
    public Order Order { get; set; }
}