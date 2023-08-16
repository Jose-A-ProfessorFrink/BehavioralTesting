namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record CustomerDataModel
{
    public Guid Id { get; init;}
    public string Name { get; init;} = default!;
    public DateOnly DateOfBirth { get; init;}
    public DateOnly DateHired { get; init;}
    public decimal AnnualSalary { get; init;}
}
