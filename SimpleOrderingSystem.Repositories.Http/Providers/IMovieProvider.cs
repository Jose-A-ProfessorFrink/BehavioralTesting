using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Repositories.Http.Providers;

internal interface IMovieProvider
{
    Task<GetMovieApiResponseDataModel> GetMovieAsync(string apiKey, string id);

    Task<SearchMoviesApiResponseDataModel> SearchMoviesAsync(string apiKey, string title);
}