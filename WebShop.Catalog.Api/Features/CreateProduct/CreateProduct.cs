using System.ComponentModel.DataAnnotations;
using Toolkit.Result;
using WebShop.Catalog.Contracts.Dtos;

namespace WebShop.Catalog.Api.Features.CreateProduct;

public sealed record CreateProduct : MediatR.IRequest<Result<ProductDto>>
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public int UnitPrice { get; set; }
}