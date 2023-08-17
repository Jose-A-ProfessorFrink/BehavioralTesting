using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.ViewModels;

public record AddOrderItemRequestViewModel
{
    [Required]
    public string MovieId {get; init;} =default!;

    public int? Quantity {get; init;}
}
