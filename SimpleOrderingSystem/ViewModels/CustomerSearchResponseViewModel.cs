namespace SimpleOrderingSystem.ViewModels;

public record CustomerSearchResponseViewModel
{
    public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
}