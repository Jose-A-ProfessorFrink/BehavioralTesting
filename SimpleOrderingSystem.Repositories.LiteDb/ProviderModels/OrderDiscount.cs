using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Repositories.LiteDB.ProviderModels;

internal record OrderDiscountDataModel
{
    public DiscountType Type { get; set; }
    public decimal PercentDiscount {get;set;}
}
