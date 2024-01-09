using System;
using Microsoft.EntityFrameworkCore;
using Toolkit.Repository.EntityFrameworkCore;

namespace WebShop.Order.Api.Infrastructure;

public sealed class OrderRepository : CrudRepository<Domain.Order, Ulid>
{
    public OrderRepository(DbContext context) : base(context)
    {
    }
}