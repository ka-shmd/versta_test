namespace VerstaDelivery.Api.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    public string Generate()
    {
        return Guid.NewGuid().ToString("N");
    }
}
