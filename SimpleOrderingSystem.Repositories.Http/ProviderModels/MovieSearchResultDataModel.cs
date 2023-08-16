namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal record MovieSearchResultDataModel
{
    public string ImdbId { get; init;}= default!;
    public string Type { get; init;}= default!;
    public string Title { get; init;}= default!;
    public string? Year { get; init;}
    public string? Poster {get; init;}
}