using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Toolkit.Endpoints;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.GetOrder;

public sealed class GetOrdersEndpoint : Endpoint.WithoutRequest.WithResponse<IList<OrderDto>>
{
    private readonly OrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersEndpoint(OrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("orders")]
    public override async Task<ActionResult<IList<OrderDto>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);
        var mappedOrders = orders.Select(o => _mapper.Map<OrderDto>(o)).ToArray();
        return mappedOrders;
    }
}