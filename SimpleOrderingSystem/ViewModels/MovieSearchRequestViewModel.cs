using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.ViewModels;

public class MovieSearchRequestViewModel
{
    [Required]
    public string? Name {get; set;}
}