using SimpleOrderingSystem.Repositories.Http.ProviderModels;

namespace SimpleOrderingSystem.Repositories.Http.Providers;

internal interface IMovieProvider
{
    Task<GetMovieApiResponse> GetMovieAsync(string apiKey, string id);

    Task<SearchMoviesApiResponse> SearchMoviesAsync(string apiKey, string title);
}