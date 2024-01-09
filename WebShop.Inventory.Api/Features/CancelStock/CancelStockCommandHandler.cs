using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Inventory.Api.Infrastructure;
using WebShop.Inventory.Contracts.Events;

namespace WebShop.Inventory.Api.Features.CancelStock;

public sealed class CancelStockCommandHandler : IHandleMessages<Contracts.Commands.CancelStock>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StockRepository _repository;
    private readonly IBus _bus;
    private readonly ILogger<CancelStockCommandHandler> _logger;

    public CancelStockCommandHandler(IUnitOfWork unitOfWork, IBus bus, ILogger<CancelStockCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<StockRepository>();
        _bus = bus;
        _logger = logger;
    }

    public async Task Handle(Contracts.Commands.CancelStock message)
    {
        _logger.LogCritical("CancelStock: {Command}", message);
        Thread.Sleep(1000);

        var productIds = message.OrderLines.Select(ol => ol.ProductId).ToArray();
        var stocks = await _repository.GetAllAsync(filter: s => productIds.Contains(s.Id));

        foreach (var stock in stocks)
        {
            var orderLine = message.OrderLines.FirstOrDefault(ol => ol.ProductId == stock.Id);
            stock.Quantity += orderLine?.Quantity ?? 0;
        }
        
        await _unitOfWork.SaveChangesAsync();

        await _bus.Publish(new StockUpdateFailed(message.OrderId));
    }
}