
namespace SimpleOrderingSystem.DataModels;

internal class SearchMoviesApiResponseDataModel
{
    public string? Response {get;set;}
    public string? Error {get;set;}
    public List<MovieSearchResultDataModel> Search {get; set;} = default!;
}