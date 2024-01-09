namespace WebShop.Order.Contracts.Dtos;

public enum OrderStatus
{
    AwaitingValidation = 0,
    StockConfirmed = 1,
    PaymentCompleted = 2,
    Completed = 3,
    Cancelled = 4,
}