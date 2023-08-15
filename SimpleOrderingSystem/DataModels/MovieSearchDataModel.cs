namespace SimpleOrderingSystem.DataModels;

internal record MovieSearchDataModel
{
    public string ImdbId { get; init;}= default!;
    public string Type { get; init;}= default!;
    public string Title { get; init;}= default!;
    public int? Year { get; init;}
    public string? Poster {get; init;}
}