namespace SimpleOrderingSystem.Tests.TestModels;


public record TestOrderSearchResponseViewModel
{
    public List<TestOrderViewModel> Orders { get; set; } = new List<TestOrderViewModel>();
}