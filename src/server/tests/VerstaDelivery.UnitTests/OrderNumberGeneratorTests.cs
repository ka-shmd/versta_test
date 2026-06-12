using Microsoft.Extensions.Time.Testing;
using VerstaDelivery.Api.Services;

namespace VerstaDelivery.UnitTests;

public class OrderNumberGeneratorTests
{
    private readonly OrderNumberGenerator _sut = new(
        new FakeTimeProvider(new DateTimeOffset(2026, 6, 12, 12, 0, 0, TimeSpan.Zero)));

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

    [Fact]
    public void GenerateOrderNumber_Called_ReturnsCorrectFormat()
    {
        var number = _sut.Generate();

        Assert.StartsWith("ORD-20260612", number);
    }
}
