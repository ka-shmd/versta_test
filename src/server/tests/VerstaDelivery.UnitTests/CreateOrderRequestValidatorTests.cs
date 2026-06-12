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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public async Task CreateOrderValidation_WeightIsInvalid_ReturnsError(decimal weight)
    {
        var request = ValidRequest() with {Weight = weight};

        var result = await _sut.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrderRequest.Weight));
    }

    [Fact]
    public async Task CreateOrderValidation_PickupDateIsInvalid_ReturnsError()
    {
        var request = ValidRequest() with
        {
            PickupDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
        };

        var result = await _sut.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrderRequest.PickupDate));
    }

    [Fact]
    public async Task CreateOrderValidation_SenderCityIsWhitespace_ReturnsSenderCityError()
    {
        var request = ValidRequest() with
        {
            SenderCity = " "
        };

        var result = await _sut.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrderRequest.SenderCity));
    }
}
