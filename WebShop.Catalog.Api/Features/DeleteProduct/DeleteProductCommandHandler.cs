using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using Toolkit.Result;
using WebShop.Catalog.Api.Infrastructure;
using WebShop.Catalog.Contracts.Events;

namespace WebShop.Catalog.Api.Features.DeleteProduct;

internal sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProduct, Result<Result>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBus bus)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<ProductRepository>();
        _mapper = mapper;
        _bus = bus;
    }

    [HttpPost("catalog/delete")]
    public async Task<Result<Result>> Handle(DeleteProduct command, CancellationToken cancellationToken)
    {
        var productId = command.ProductId;
        var product = await _repository.GetByIdAsync(productId, cancellationToken);

        if (product is null)
        {
            return new HttpError(HttpStatusCode.BadRequest, "Not found", $"The product with id {productId} could not be found");
        }

        await _repository.DeleteByIdAsync(productId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _bus.Publish(new ProductDeleted(productId));

        return Result.Success();
    }
}