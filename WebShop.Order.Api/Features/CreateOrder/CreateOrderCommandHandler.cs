using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using Toolkit.Result;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;
using WebShop.Order.Contracts.Events;

namespace WebShop.Order.Api.Features.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrder, Result<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBus bus,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<OrderRepository>();
        _mapper = mapper;
        _bus = bus;
        _logger = logger;
    }

    [HttpPost("orders/create")]
    public async Task<Result<OrderDto>> Handle(CreateOrder command, CancellationToken cancellationToken)
    {
        _logger.LogCritical("CreateOrder: {Command}", command);
        Thread.Sleep(1000);

        var order = _mapper.Map<Domain.Order>(command);

        await _repository.InsertAsync(order, cancellationToken);
        if (await _unitOfWork.SaveChangesAsync(cancellationToken) == false)
        {
            return new HttpError(HttpStatusCode.BadRequest, "Could not create order");
        }

        var orderDto = _mapper.Map<OrderDto>(order);

        await _bus.Send(new OrderCreated(orderDto.OrderId, orderDto.OrderLines));

        return orderDto;
    }
}