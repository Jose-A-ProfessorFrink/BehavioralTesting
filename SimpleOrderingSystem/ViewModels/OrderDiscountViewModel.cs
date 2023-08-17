using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.ViewModels;

public class OrderDiscountViewModel
{
    public DiscountType Type { get; set; }
    public decimal PercentDiscount {get;set;}
}