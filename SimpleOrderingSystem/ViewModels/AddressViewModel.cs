namespace SimpleOrderingSystem.ViewModels;

public record AddressViewModel
{
    public string Line1 { get; init;} = default!;
    public string Line2 { get; init;} = default!;
    public string City { get; init;} = default!;
    public string State { get; init;} = default!;
    public string ZipCode { get; init;} = default!;
}
