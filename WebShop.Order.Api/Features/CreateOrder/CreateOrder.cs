using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toolkit.Result;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.CreateOrder;

public sealed record CreateOrder : MediatR.IRequest<Result<OrderDto>>
{
    [Required]
    public string AddressLine { get; set; } = string.Empty;

    [MinLength(1)]
    public IList<OrderLineDto> OrderLines { get; set; } = Array.Empty<OrderLineDto>();
}