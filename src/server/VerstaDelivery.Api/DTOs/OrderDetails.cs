namespace VerstaDelivery.Api.DTOs;

public record OrderDetails(
    string OrderNumber,
    string SenderCity,
    string SenderAddress,
    string RecipientCity,
    string RecipientAddress,
    decimal Weight,
    DateOnly PickupDate,
    DateTime CreatedAt);
