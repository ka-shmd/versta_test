namespace VerstaDelivery.Api.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string SenderCity { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
    public string RecipientCity { get; set; } = null!;
    public string RecipientAddress { get; set; } = null!;
    public decimal Weight { get; set; }
    public DateOnly PickupDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
