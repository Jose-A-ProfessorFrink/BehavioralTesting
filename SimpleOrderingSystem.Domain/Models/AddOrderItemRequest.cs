
namespace SimpleOrderingSystem.Domain.Models;

public record AddOrderItemRequest
{
    public string OrderId { get; init; } = default!;
    
    public string MovieId {get; init;} =default!;

    public int Quantity {get; init;}
}
