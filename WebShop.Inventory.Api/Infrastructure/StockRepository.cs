using System;
using Microsoft.EntityFrameworkCore;
using Toolkit.Repository.EntityFrameworkCore;

namespace WebShop.Inventory.Api.Infrastructure;

public sealed class StockRepository : CrudRepository<Domain.Stock, Ulid>
{
    public StockRepository(DbContext context) : base(context)
    {
    }
}