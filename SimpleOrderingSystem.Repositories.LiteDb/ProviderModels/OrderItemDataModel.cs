namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record OrderItemDataModel
{
    public Guid Id {get; init;}
    public string MovieId { get; init;} = default!;
    public string MovieTitle { get; init;} = default!;
    public decimal Price { get; init;} = default!;
    public int Quantity { get; init;}
}