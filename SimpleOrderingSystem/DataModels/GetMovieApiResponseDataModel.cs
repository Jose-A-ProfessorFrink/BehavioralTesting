namespace SimpleOrderingSystem.DataModels;

internal record GetMovieApiResponseDataModel
{
    public string ImdbId { get; init;} = default!;
    public string Type { get; init;} = default!;
    public string Title { get; init;} = default!;
    public string? Year { get; init;}
    public string? Rated { get; init;}
    public string? Plot {get; init;}
    public string? Released { get; init;}
    public string? Poster {get; init;}
    public string? Metascore {get; init;}
    public string? ImdbRating {get; init;}

    public string? Response {get; init;}
    public string? Error {get; init;}
}
