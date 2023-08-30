namespace SimpleOrderingSystem.Tests.TestModels;

public record TestCustomerSearchResponseViewModel
{
    public List<TestCustomerViewModel> Customers { get; set; } = new List<TestCustomerViewModel>();
}