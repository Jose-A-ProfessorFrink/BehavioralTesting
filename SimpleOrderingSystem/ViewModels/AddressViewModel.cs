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
    [RegularExpression(@"^([Aa][LKSZRAEPlkszraep]|[Cc][AOTaot]|[Dd][ECec]|[Ff][LMlm]|[Gg][AUau]|[Hh][Ii]|[Ii][ADLNadln]|[Kk][SYsy]|[Ll][Aa]|[Mm][ADEHINOPSTadehinopst]|[Nn][CDEHJMVYcdehjmvy]|[Oo][HKRhkr]|[Pp][ARWarw]|[Rr][Ii]|[Ss][CDcd]|[Tt][NXnx]|[Uu][Tt]|[Vv][AITait]|[Ww][AIVYaivy])$", 
        ErrorMessage = "Invalid state supplied. Please supply a two valid two digit code!")]
    public string State { get; init;} = default!;
    [Required]
    [RegularExpression(@"^(?!00000)[0-9]{5,5}$", ErrorMessage = "Zip code should contain 5 digits")]
    public string ZipCode { get; init;} = default!;
}
