namespace SimpleOrderingSystem.Tests.TestModels;

public record TestMovieSearchResponseViewModel
{
    public List<TestMovieSearchViewModel> Movies { get; set; } = new List<TestMovieSearchViewModel>();
}