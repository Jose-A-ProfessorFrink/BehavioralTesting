
namespace SimpleOrderingSystem.Repositories.Http.ProviderModels;

internal class SearchMoviesApiResponse
{
    public string? Response {get;set;}
    public string? Error {get;set;}
    public List<SearchMoviesResult> Search {get; set;} = default!;
}