namespace SimpleOrderingSystem.Tests.TestModels;

public record TestOrderViewModel
{
    public Guid Id { get; init;}
    public string Status { get; init;} = default!;
    public string Type { get; init;} = default!;
    public TestCustomerViewModel Customer { get; init;} = default!;
    public TestAddressViewModel? ShippingAddress { get; init;} 
    public List<TestOrderItemViewModel> Items { get; init;} = new List<TestOrderItemViewModel>();
    public decimal Shipping { get; init;}
    public decimal LineItemTotal { get; init;}
    public decimal DiscountTotal { get; init;}
    public decimal TotalCost { get; init;}
    public List<TestOrderDiscountViewModel> Discounts {get; init;} = new List<TestOrderDiscountViewModel>();
    public DateTime CreatedDateTimeUtc { get; init;}
    public DateTime? CancelledDateTimeUtc {get; init;}
    public DateTime? CompletedDateTimeUtc {get; init;}
}