using VerstaDelivery.Api.DTOs;

namespace VerstaDelivery.Api.Models;

public static class OrderExtensions
{
    public static OrderDetails ToDetails(this Order order) => new OrderDetails(order.OrderNumber, order.SenderCity,
        order.SenderAddress, order.RecipientCity, order.RecipientAddress, order.Weight, order.PickupDate, order.CreatedAt);
}
