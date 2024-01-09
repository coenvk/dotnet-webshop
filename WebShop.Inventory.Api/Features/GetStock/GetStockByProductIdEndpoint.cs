using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Toolkit.Endpoints;
using WebShop.Inventory.Api.Infrastructure;
using WebShop.Inventory.Contracts.Dtos;

namespace WebShop.Inventory.Api.Features.GetStock;

public sealed class GetStockByProductIdEndpoint : Endpoint.WithRequest<Ulid>.WithResponse<StockDto>
{
    private readonly StockRepository _repository;
    private readonly IMapper _mapper;

    public GetStockByProductIdEndpoint(StockRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("stock/{productId}")]
    public override async Task<ActionResult<StockDto>> HandleAsync(Ulid productId,
        CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByIdAsync(productId, cancellationToken)
                        ?? throw new ArgumentException(nameof(productId));

        return _mapper.Map<StockDto>(stock);
    }
}