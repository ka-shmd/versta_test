using VerstaDelivery.Api.Services;

namespace VerstaDelivery.UnitTests;

public class OrderNumberGeneratorTests
{
    private readonly OrderNumberGenerator _sut = new();

    [Fact]
    public void GenerateOrderNumber_CalledOnce_ReturnsCorrectFormat()
    {
        var number = _sut.Generate();
        Assert.Matches(@"^ORD-\d{8}-[23456789ABCDEFGHJKMNPQRSTUVWXYZ]{6}$", number);
    }

    [Fact]
    public void GenerateOrderNumber_CalledTwice_ReturnsDifferentNumbers()
    {
        var firstNumber = _sut.Generate();
        var secondNumber = _sut.Generate();
        Assert.NotEqual(firstNumber, secondNumber);
    }
}
