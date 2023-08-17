
namespace SimpleOrderingSystem.Domain.Models;

public record DeleteOrderItemRequest
{
    public string OrderId { get; init; } = default!;
    
    public string MovieId {get; init;} =default!;
}
