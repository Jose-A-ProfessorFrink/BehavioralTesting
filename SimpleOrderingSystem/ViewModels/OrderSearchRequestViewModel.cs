using System.ComponentModel.DataAnnotations;

namespace SimpleOrderingSystem.Domain.Models;

public record OrderSearchRequestViewModel
{
    public Guid? CustomerId { get; init;}
    public DateTime? NoOlderThan {get;init;}
}