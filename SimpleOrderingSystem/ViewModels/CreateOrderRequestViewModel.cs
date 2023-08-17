using System.ComponentModel.DataAnnotations;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.ViewModels;

public record CreateOrderRequestViewModel
{
    [Required]
    public string? CustomerId { get; init;}

    [Required]
    public OrderType Type { get; init; }

    [Required]
    public AddressViewModel? ShippingAddress {get; init;}
}