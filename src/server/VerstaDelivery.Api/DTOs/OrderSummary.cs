namespace VerstaDelivery.Api.DTOs;

public record OrderSummary(
    string OrderNumber,
    string SenderCity,
    string SenderAddress,
    string RecipientCity,
    string RecipientAddress,
    decimal Weight,
    DateOnly PickupDate);
