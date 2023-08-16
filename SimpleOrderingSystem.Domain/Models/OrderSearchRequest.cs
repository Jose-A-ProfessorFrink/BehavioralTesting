namespace SimpleOrderingSystem.Domain.Models;

public class OrderSearchRequest
{
    public Guid? CustomerId { get; init;}
    public DateTime? NoOlderThan {get;init;}
}