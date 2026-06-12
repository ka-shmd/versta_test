namespace VerstaDelivery.Api.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    private const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    private const int SuffixLength = 6;

    private readonly TimeProvider _timeProvider;

    public OrderNumberGenerator(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public string Generate()
    {
        var date = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        var suffix = RandomSuffix();
        return $"ORD-{date:yyyyMMdd}-{suffix}";
    }

    private string RandomSuffix()
    {
        Span<char> buffer = stackalloc char[SuffixLength];
        for (var i = 0; i < SuffixLength; i++)
        {
            buffer[i] = Alphabet[Random.Shared.Next(0, Alphabet.Length)];
        }

        return new string(buffer);
    }
}
