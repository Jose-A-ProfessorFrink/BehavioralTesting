namespace SimpleOrderingSystem.DataModels;

internal record MovieDataModel
{
    public string ImdbId { get; init;} = default!;
    public string Type { get; init;} = default!;
    public string Title { get; init;} = default!;
    public int? Year { get; init;}
    public string? Rated { get; init;}
    public string? Plot {get; init;}
    public string? Released { get; init;}
    public string? Poster {get; init;}
    public string? Metascore {get; init;}
    public string? ImdbRating {get; init;}
}
