using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using Toolkit.Result;
using WebShop.Catalog.Api.Infrastructure;
using WebShop.Catalog.Contracts.Dtos;

namespace WebShop.Catalog.Api.Features.UpdateProduct;

internal sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProduct, Result<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductRepository _repository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = _unitOfWork.Repository<ProductRepository>();
        _mapper = mapper;
    }

    [HttpPost("catalog/update")]
    public async Task<Result<ProductDto>> Handle(UpdateProduct command, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(command.ProductId, cancellationToken)
                      ?? throw new ArgumentException(nameof(command.ProductId));

        _mapper.Map(command, product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}