using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.CompleteOrder;

public sealed class CompleteOrderCommandHandler : IHandleMessages<Contracts.Commands.CompleteOrder>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderRepository _repository;
    private readonly ILogger<CompleteOrderCommandHandler> _logger;

    public CompleteOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CompleteOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<OrderRepository>();
        _logger = logger;
    }

    public async Task Handle(Contracts.Commands.CompleteOrder message)
    {
        _logger.LogCritical("CompleteOrder: {Event}", message);

        var order = await _repository.GetFirstAsync(filter: o => o.Id == message.OrderId);

        order!.Status = OrderStatus.Completed;

        await _unitOfWork.SaveChangesAsync();
    }
}