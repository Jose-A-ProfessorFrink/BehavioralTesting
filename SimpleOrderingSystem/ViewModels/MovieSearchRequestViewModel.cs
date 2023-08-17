using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.ViewModels;

public record MovieSearchRequestViewModel
{
    [Required]
    public string? Name {get; set;}
}