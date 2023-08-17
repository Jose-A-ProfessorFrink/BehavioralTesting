namespace SimpleOrderingSystem.ViewModels;

public record MovieSearchResponseViewModel
{
    public List<MovieSearchViewModel> Movies { get; set; } = new List<MovieSearchViewModel>();
}