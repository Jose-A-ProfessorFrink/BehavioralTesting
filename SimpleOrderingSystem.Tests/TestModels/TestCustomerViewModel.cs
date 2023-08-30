namespace SimpleOrderingSystem.Tests.TestModels;

public class TestCustomerViewModel
{
    public Guid Id { get; set;}
    public string Name { get; set;} = default!;
    public DateOnly DateOfBirth { get; set;}
    public DateOnly DateHired { get; set;}
    public decimal AnnualSalary { get; set;}
}