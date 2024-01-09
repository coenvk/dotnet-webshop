using System.Threading.Tasks;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Inventory.Contracts.Events;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Dtos;
using WebShop.Payment.Contracts.Events;

namespace WebShop.Order.Api.Features.OrderSaga;

public sealed class OrderSagaEventListener :
    IHandleMessages<StockUpdated>,
    IHandleMessages<PaymentCompleted>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OrderRepository _repository;

    public OrderSagaEventListener(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<OrderRepository>();
    }

    public async Task Handle(StockUpdated message)
    {
        var order = await _repository.GetFirstAsync(filter: o => o.Id == message.OrderId);

        order!.Status = OrderStatus.StockConfirmed;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task Handle(PaymentCompleted message)
    {
        var order = await _repository.GetFirstAsync(filter: o => o.Id == message.OrderId);

        order!.Status = OrderStatus.PaymentCompleted;

        await _unitOfWork.SaveChangesAsync();
    }
}