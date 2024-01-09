using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using Toolkit.Result;
using WebShop.Catalog.Api.Domain;
using WebShop.Catalog.Api.Infrastructure;
using WebShop.Catalog.Contracts.Dtos;
using WebShop.Catalog.Contracts.Events;

namespace WebShop.Catalog.Api.Features.CreateProduct;

internal sealed class CreateProductCommandHandler : IRequestHandler<CreateProduct, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBus bus)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<ProductRepository>();
        _mapper = mapper;
        _bus = bus;
    }

    [HttpPost("catalog/create")]
    public async Task<Result<ProductDto>> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(command);

        await _repository.InsertAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _bus.Publish(new ProductCreated(product.Id));

        return _mapper.Map<ProductDto>(product);
    }
}