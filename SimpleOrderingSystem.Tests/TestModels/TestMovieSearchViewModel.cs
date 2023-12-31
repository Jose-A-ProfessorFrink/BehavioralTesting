namespace SimpleOrderingSystem.Tests.TestModels;

public record TestMovieSearchViewModel
{
    public string Id { get; set;} = default!;
    public string Type { get; init;} = default!;
    public string Title { get; init;} = default!;
    public string? Year { get; init;}
    public string? PosterUrl {get; init;}
}