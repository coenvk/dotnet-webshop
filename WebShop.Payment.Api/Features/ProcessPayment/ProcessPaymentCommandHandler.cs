using Rebus.Bus;
using Rebus.Handlers;
using WebShop.Payment.Contracts.Events;

namespace WebShop.Payment.Api.Features.ProcessPayment;

public sealed class ProcessPaymentCommandHandler : IHandleMessages<Contracts.Commands.ProcessPayment>
{
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly IBus _bus;

    public ProcessPaymentCommandHandler(ILogger<ProcessPaymentCommandHandler> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public Task Handle(Contracts.Commands.ProcessPayment message)
    {
        _logger.LogCritical("ProcessPayment: {Command}", message);
        
        var orderId = message.OrderId;

        _logger.LogInformation("Payment being processed for order {OrderId}", orderId);
        Thread.Sleep(1000);

        var randomInt = Random.Shared.Next(5);

        if (randomInt == 0)
        {
            _logger.LogInformation("Payment failed for order {OrderId}", orderId);
            _bus.Publish(new PaymentFailed(orderId));
        }
        else
        {
            _logger.LogInformation("Payment completed successfully for order {OrderId}", orderId);
            _bus.Publish(new PaymentCompleted(orderId));
        }

        return Task.CompletedTask;
    }
}