using System;
using System.ComponentModel.DataAnnotations;
using Toolkit.Result;

namespace WebShop.Catalog.Api.Features.DeleteProduct;

public sealed record DeleteProduct : MediatR.IRequest<Result<Result>>
{
    [Required]
    public Ulid ProductId { get; set; }
}