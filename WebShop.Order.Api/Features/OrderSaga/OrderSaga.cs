using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using WebShop.Inventory.Contracts.Commands;
using WebShop.Inventory.Contracts.Events;
using WebShop.Order.Contracts.Events;
using WebShop.Payment.Contracts.Commands;
using WebShop.Payment.Contracts.Events;

namespace WebShop.Order.Api.Features.OrderSaga;

public sealed class OrderSaga : Saga<OrderSagaData>,
    IAmInitiatedBy<OrderCreated>,
    IHandleMessages<StockUpdated>,
    IHandleMessages<StockUpdateFailed>,
    IHandleMessages<PaymentCompleted>,
    IHandleMessages<PaymentFailed>,
    IHandleMessages<OrderCompleted>,
    IHandleMessages<OrderCancelled>
{
    private readonly IBus _bus;
    private readonly ILogger<OrderSaga> _logger;

    public OrderSaga(IBus bus, ILogger<OrderSaga> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override void CorrelateMessages(ICorrelationConfig<OrderSagaData> config)
    {
        config.Correlate<OrderCreated>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<StockUpdated>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<StockUpdateFailed>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<PaymentCompleted>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<PaymentFailed>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<OrderCompleted>(m => m.OrderId.ToString(), s => s.OrderId);
        config.Correlate<OrderCancelled>(m => m.OrderId.ToString(), s => s.OrderId);
    }

    public async Task Handle(OrderCreated message)
    {
        _logger.LogCritical("OrderCreated: {Event}", message);
        
        if (!IsNew)
        {
            return;
        }

        Data.OrderLines = message.OrderLines;
        await _bus.Send(new UpdateStock(message.OrderId, message.OrderLines));
    }

    public async Task Handle(StockUpdated message)
    {
        _logger.LogCritical("StockUpdated: {Event}", message);

        if (Data.StockUpdated)
        {
            return;
        }

        Data.StockUpdated = true;
        await _bus.Send(new ProcessPayment(message.OrderId));
    }

    public async Task Handle(PaymentCompleted message)
    {
        _logger.LogCritical("PaymentCompleted: {Event}", message);

        if (Data.PaymentCompleted)
        {
            return;
        }

        Data.PaymentCompleted = true;
        await _bus.Send(new Contracts.Commands.CompleteOrder(message.OrderId));
    }

    public Task Handle(OrderCompleted message)
    {
        _logger.LogCritical("OrderCompleted: {Event}", message);

        MarkAsComplete();
        return Task.CompletedTask;
    }

    public async Task Handle(PaymentFailed message)
    {
        _logger.LogCritical("PaymentFailed: {Event}", message);

        Data.PaymentCompleted = false;

        if (Data.StockUpdated)
        {
            await _bus.Send(new CancelStock(message.OrderId, Data.OrderLines));
        }
    }

    public async Task Handle(StockUpdateFailed message)
    {
        _logger.LogCritical("StockUpdateFailed: {Event}", message);

        Data.StockUpdated = false;
        await _bus.Send(new Contracts.Commands.CancelOrder(message.OrderId));
    }

    public Task Handle(OrderCancelled message)
    {
        _logger.LogCritical("OrderCancelled: {Event}", message);

        MarkAsComplete();
        return Task.CompletedTask;
    }
}