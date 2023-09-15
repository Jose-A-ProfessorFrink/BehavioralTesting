namespace SimpleOrderingSystem.Tests.TestModels;

public class TestCreateOrderRequestViewModel
{
    public string? CustomerId { get; set;}

    public string? Type { get; set; }

    public TestAddressViewModel? ShippingAddress {get; set;}
}