namespace SimpleOrderingSystem.ViewModels;

public class OrderViewModel
{
    public Guid Id { get; init;}
    public string Status { get; init;} = default!;
    public string Type { get; init;} = default!;
    public CustomerViewModel Customer { get; init;} = default!;
    public AddressViewModel? ShippingAddress { get; init;} 
    //public List<OrderItemDataModel> Items { get; init;} = new List<OrderItemDataModel>();
    public decimal Shipping { get; init;}
    public decimal TotalCost { get; init;}
    //public List<OrderDiscount> AppliedDiscounts {get; init;} = new List<OrderDiscount>();
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; init;}
    public DateTime? CompletedDateTimeUtc {get; init;}
}