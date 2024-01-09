using System;
using System.Collections.Generic;
using Rebus.Sagas;
using WebShop.Order.Contracts.Dtos;

namespace WebShop.Order.Api.Features.OrderSaga;

public sealed class OrderSagaData : ISagaData
{
    public Guid Id { get; set; }
    public int Revision { get; set; }

    public string OrderId { get; set; } = string.Empty;
    public bool StockUpdated { get; set; }
    public bool PaymentCompleted { get; set; }

    public IList<OrderLineDto> OrderLines { get; set; } = Array.Empty<OrderLineDto>();
}