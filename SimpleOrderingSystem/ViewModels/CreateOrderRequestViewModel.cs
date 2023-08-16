using System.ComponentModel.DataAnnotations;
using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.ViewModels;

public class CreateOrderRequestViewModel
{
    [Required]
    public string? CustomerId { get; init;}

    [Required]
    public OrderType Type { get; init; }

    [Required]
    public AddressViewModel? ShippingAddress {get; init;}
}