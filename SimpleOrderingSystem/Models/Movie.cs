namespace SimpleOrderingSystem.Models;

public class Movie
{
    public string Id { get; set;} = default!;
    public string Type { get; init;} = default!;
    public string Title { get; init;} = default!;
    public string? Year { get; init;}
    public string? Rating { get; init;}
    public string? PlotDescription {get; init;}
    public string? DateReleased { get; init;}
    public string? PosterUrl {get; init;}
    public string? Metascore {get; init;}
    public string? ImdbRating {get; init;}
}
