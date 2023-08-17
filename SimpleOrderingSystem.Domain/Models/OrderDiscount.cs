namespace SimpleOrderingSystem.Domain.Models;

public class OrderDiscount
{
    public DiscountType Type { get; set; }
    public decimal PercentDiscount {get;set;}
}

