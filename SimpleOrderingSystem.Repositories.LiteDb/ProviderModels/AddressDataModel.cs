namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record AddressDataModel
{
    public string Line1 { get; init;} = default!;
    public string? Line2 { get; init;}
    public string City { get; init;} = default!;
    public string State { get; init;} = default!;
    public string ZipCode { get; init;} = default!;
}

