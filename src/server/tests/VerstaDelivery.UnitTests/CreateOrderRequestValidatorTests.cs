using VerstaDelivery.Api.DTOs;
using VerstaDelivery.Api.Validation;

namespace VerstaDelivery.UnitTests;

public class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _sut = new();

    private CreateOrderRequest ValidRequest() => new(
        "City",
        "Address",
        "City",
        "Address",
        10,
        DateOnly.FromDateTime(DateTime.UtcNow));

    [Fact]
    public async Task CreateOrderValidation_RequestIsValid_ReturnsNoErrors()
    {
        var result = await _sut.ValidateAsync(ValidRequest());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreateOrderValidation_StringFieldsAreEmpty_ReturnsErrorsForAllFields()
    {
        var request = ValidRequest() with
        {
            SenderCity = string.Empty,
            SenderAddress = string.Empty,
            RecipientCity = string.Empty,
            RecipientAddress = string.Empty
        };

        var result = await _sut.ValidateAsync(request);

        Assert.False(result.IsValid);

        var errorProperties = result.Errors.Select(e => e.PropertyName).ToHashSet();

        string[] expectedEmptyFieldErrors =
        [
            nameof(CreateOrderRequest.SenderCity),
            nameof(CreateOrderRequest.SenderAddress),
            nameof(CreateOrderRequest.RecipientCity),
            nameof(CreateOrderRequest.RecipientAddress)
        ];

        Assert.All(expectedEmptyFieldErrors, errorProperty => Assert.Contains(errorProperty, errorProperties));
    }
}
