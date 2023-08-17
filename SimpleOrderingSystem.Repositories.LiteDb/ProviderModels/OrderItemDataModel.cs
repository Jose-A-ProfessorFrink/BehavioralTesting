namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record OrderItemDataModel
{
    public string MovieId { get; set; } = default!;
    public string? MovieYear {get; set; }
    public string? MovieMetascore {get; set; }
    public int Quantity {get; set; }
    public decimal Price {get;set;}
}