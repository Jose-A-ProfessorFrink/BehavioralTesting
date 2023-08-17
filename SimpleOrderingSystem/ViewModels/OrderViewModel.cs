namespace SimpleOrderingSystem.ViewModels;

public record OrderViewModel
{
    public Guid Id { get; init;}
    public string Status { get; init;} = default!;
    public string Type { get; init;} = default!;
    public CustomerViewModel Customer { get; init;} = default!;
    public AddressViewModel? ShippingAddress { get; init;} 
    public List<OrderItemViewModel> Items { get; init;} = new List<OrderItemViewModel>();
    public decimal Shipping { get; init;}
    public decimal TotalCost { get; init;}
    public List<OrderDiscountViewModel> Discounts {get; init;} = new List<OrderDiscountViewModel>();
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; init;}
    public DateTime? CompletedDateTimeUtc {get; init;}
}