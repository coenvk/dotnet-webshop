using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Toolkit.Domain.Entities;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Domain;

[Table("order")]
public sealed class Order : AggregateRoot<Ulid>
{
    public Order() : base(Ulid.NewUlid())
    {
    }

    public string AddressLine { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }

    public IList<OrderLine> OrderLines { get; set; } = Array.Empty<OrderLine>();
}