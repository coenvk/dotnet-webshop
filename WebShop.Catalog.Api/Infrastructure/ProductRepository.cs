using System;
using Microsoft.EntityFrameworkCore;
using Toolkit.Repository.EntityFrameworkCore;
using WebShop.Catalog.Api.Domain;

namespace WebShop.Catalog.Api.Infrastructure;

public sealed class ProductRepository : CrudRepository<Product, Ulid>
{
    public ProductRepository(DbContext context) : base(context)
    {
    }
}