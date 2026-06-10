namespace VerstaDelivery.Api.DTOs;

public record CreateOrderRequest(
    string SenderCity,
    string SenderAddress,
    string RecipientCity,
    string RecipientAddress,
    decimal Weight,
    DateOnly PickupDate);
