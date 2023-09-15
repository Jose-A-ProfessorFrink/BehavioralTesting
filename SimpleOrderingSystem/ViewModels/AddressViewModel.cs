using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.ViewModels;

public record AddressViewModel
{
    [Required]
    public string Line1 { get; init;} = default!;
    public string? Line2 { get; init;}
    [Required]
    public string City { get; init;} = default!;
    [Required]
    [RegularExpression(@"^([A][L]|[C][A]|[O][R]|[W][A])$", 
        ErrorMessage = "Invalid state supplied. Only states supported values are AL, CA, OR and WA.")]
    public string State { get; init;} = default!;
    [Required]
    [RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Zip code should contain 5 digits")]
    public string ZipCode { get; init;} = default!;
}
