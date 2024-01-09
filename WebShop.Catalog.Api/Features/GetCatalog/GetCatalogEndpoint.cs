using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Toolkit.Endpoints;
using WebShop.Catalog.Api.Infrastructure;
using WebShop.Catalog.Contracts.Dtos;

namespace WebShop.Catalog.Api.Features.GetCatalog;

public sealed class GetCatalogEndpoint : Endpoint.WithoutRequest.WithResponse<IList<ProductDto>>
{
    private readonly ProductRepository _repository;
    private readonly IMapper _mapper;

    public GetCatalogEndpoint(ProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("catalog")]
    public override async Task<ActionResult<IList<ProductDto>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var catalog = await _repository.GetAllAsync(cancellationToken);
        var productList = _mapper.Map<IList<ProductDto>>(catalog.ToList());
        return new ActionResult<IList<ProductDto>>(productList);
    }
}