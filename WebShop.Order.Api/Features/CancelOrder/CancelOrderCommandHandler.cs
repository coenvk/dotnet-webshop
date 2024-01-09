using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.CancelOrder;

public sealed class CancelOrderCommandHandler : IHandleMessages<Contracts.Commands.CancelOrder>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderRepository _repository;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<OrderRepository>();
        _logger = logger;
    }

    public async Task Handle(Contracts.Commands.CancelOrder message)
    {
        _logger.LogCritical("CancelOrder: {Event}", message);

        var order = await _repository.GetFirstAsync(filter: o => o.Id == message.OrderId);

        order!.Status = OrderStatus.Cancelled;

        await _unitOfWork.SaveChangesAsync();
    }
}