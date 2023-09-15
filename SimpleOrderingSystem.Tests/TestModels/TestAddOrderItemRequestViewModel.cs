namespace SimpleOrderingSystem.Tests.TestModels;

public record TestAddOrderItemRequestViewModel
{
    public string MovieId {get; init;} =default!;

    public int? Quantity {get; init;}
}
