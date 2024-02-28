namespace SimpleOrderingSystem.Domain.Models;

public class OrderItem
{
    public string MovieId { get; init; } = default!;
    public string? MovieYear {get; init; }
    public string? MovieMetascore {get; init; }
    public int Quantity {get; set; }
    public decimal Price {get;init;}
}