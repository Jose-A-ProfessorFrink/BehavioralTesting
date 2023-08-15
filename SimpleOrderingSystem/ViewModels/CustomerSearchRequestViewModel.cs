using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.ViewModels;

public class CustomerSearchRequestViewModel
{
    [Required]
    [MinLength(2, ErrorMessage = "The name search must be at least 2 characters long.")]
    [MaxLength(100, ErrorMessage ="The name search must not exceed 100 characters in length.")]
    public string? Name {get; set;}
}