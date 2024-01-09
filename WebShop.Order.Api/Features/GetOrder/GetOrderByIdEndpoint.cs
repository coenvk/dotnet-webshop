using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Toolkit.Endpoints;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.GetOrder;

public sealed class GetOrderByIdEndpoint : Endpoint.WithRequest<Ulid>.WithResponse<OrderDto>
{
    private readonly OrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrderByIdEndpoint(OrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("orders/{orderId}")]
    public override async Task<ActionResult<OrderDto>> HandleAsync(Ulid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
                    ?? throw new ArgumentException(nameof(orderId));

        return _mapper.Map<OrderDto>(order);
    }
}