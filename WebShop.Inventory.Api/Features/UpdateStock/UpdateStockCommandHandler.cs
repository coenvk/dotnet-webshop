using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Inventory.Api.Infrastructure;
using WebShop.Inventory.Contracts.Events;

namespace WebShop.Inventory.Api.Features.UpdateStock;

public sealed class UpdateStockCommandHandler : IHandleMessages<Contracts.Commands.UpdateStock>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StockRepository _repository;
    private readonly IBus _bus;
    private readonly ILogger<UpdateStockCommandHandler> _logger;

    public UpdateStockCommandHandler(IUnitOfWork unitOfWork, IBus bus, ILogger<UpdateStockCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<StockRepository>();
        _bus = bus;
        _logger = logger;
    }

    public async Task Handle(Contracts.Commands.UpdateStock message)
    {
        _logger.LogCritical("UpdateStock: {Command}", message);
        Thread.Sleep(1000);

        var productIds = message.OrderLines.Select(ol => ol.ProductId).ToArray();
        var stocks = await _repository.GetAllAsync(filter: s => productIds.Contains(s.Id));

        foreach (var stock in stocks)
        {
            var orderLine = message.OrderLines.FirstOrDefault(ol => ol.ProductId == stock.Id);
            stock.Quantity -= orderLine?.Quantity ?? 0;

            if (stock.Quantity < 0)
            {
                await _bus.Publish(new StockUpdateFailed(message.OrderId));
                return;
            }
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch
        {
            await _bus.Publish(new StockUpdateFailed(message.OrderId));
            return;
        }

        await _bus.Publish(new StockUpdated(message.OrderId));
    }
}