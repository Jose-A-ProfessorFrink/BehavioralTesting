namespace SimpleOrderingSystem.Models;

public class Customer
{
    public Guid Id { get; init;}
    public string Name { get; init;} = default!;
    public DateOnly DateOfBirth { get; init;}
    public DateOnly DateHired { get; init;}
    public decimal AnnualSalary { get; init;}
}
