using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.Domain.Models;

public class OrderSearchRequestViewModel
{
    public Guid? CustomerId { get; init;}
    public DateTime? NoOlderThan {get;init;}
}