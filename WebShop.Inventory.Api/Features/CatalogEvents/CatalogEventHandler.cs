using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using WebShop.Catalog.Contracts.Events;
using WebShop.Inventory.Api.Domain;
using WebShop.Inventory.Api.Infrastructure;

namespace WebShop.Inventory.Api.Features.CatalogEvents;

public sealed class CatalogEventHandler :
    IHandleMessages<ProductCreated>,
    IHandleMessages<ProductDeleted>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StockRepository _repository;
    private readonly IBus _bus;
    private readonly ILogger<CatalogEventHandler> _logger;

    public CatalogEventHandler(IUnitOfWork unitOfWork, IBus bus, ILogger<CatalogEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<StockRepository>();
        _bus = bus;
        _logger = logger;
    }

    public async Task Handle(ProductCreated message)
    {
        _logger.LogCritical("ProductCreated: {Event}", message);

        var newStock = new Stock(message.ProductId);
        newStock.Restock(10);

        await _repository.InsertAsync(newStock);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task Handle(ProductDeleted message)
    {
        _logger.LogCritical("ProductDeleted: {Event}", message);

        await _repository.DeleteByIdAsync(message.ProductId);
        await _unitOfWork.SaveChangesAsync();
    }
}