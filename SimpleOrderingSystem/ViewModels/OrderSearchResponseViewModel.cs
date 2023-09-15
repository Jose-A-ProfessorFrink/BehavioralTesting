namespace SimpleOrderingSystem.ViewModels;

public record OrderSearchResponseViewModel
{
    public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
}