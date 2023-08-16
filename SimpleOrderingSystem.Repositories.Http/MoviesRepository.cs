using SimpleOrderingSystem.Domain.Extensions;
using SimpleOrderingSystem.Repositories.Http.ProviderModels;
using SimpleOrderingSystem.Repositories.Http.Providers;
using Microsoft.Extensions.Configuration;
using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Repositories;

namespace SimpleOrderingSystem.Repositories.Http;

internal class MoviesRepository:IMoviesRepository
{
    private readonly IMovieProvider _movieProvider;
    private readonly IConfiguration _configuration;
    private const string InvalidResponseCode = "False";

    public MoviesRepository(IMovieProvider movieProvider,IConfiguration configuration)
    {
        _movieProvider = movieProvider;
        _configuration = configuration;
    }

    public async Task<Movie?> GetMovieAsync(string id)
    {
        var apiResponse = await _movieProvider.GetMovieAsync(ApiKey, id);

        if(apiResponse.Response == InvalidResponseCode && apiResponse.Error == "Incorrect IMDb ID.")
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.MovieIdInvalid);
        }

        return Map(apiResponse);
    }

    public async Task<List<MovieSummary>> SearchMoviesAsync(string title)
    {
        var apiResponse = await _movieProvider.SearchMoviesAsync(ApiKey, title);

        if(apiResponse.Response == InvalidResponseCode && apiResponse.Error == "Too many results.")
        {
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.SearchMovieRequestTooBroad);
        }

        return apiResponse.Search.Select(a=>Map(a)).ToList();
    }

    #region Helpers

    private string ApiKey => _configuration.GetRequiredValue("OmdbApiKey");

    private MovieSummary Map(SearchMoviesResult movie)
    {
        return new()
        {
            Id = movie.ImdbId,
            Type = movie.Type,
            Title = movie.Title,
            Year = movie.Year,
            PosterUrl = movie.Poster
        };
    }

    private Movie Map(GetMovieApiResponse movie)
    {
        return new()
        {
            Id = movie.ImdbId,
            Type = movie.Type,
            Title = movie.Title,
            Year = movie.Year,
            Rating = movie.Rated,
            PlotDescription = movie.Plot,
            DateReleased = movie.Released,
            PosterUrl = movie.Poster,
            Metascore = movie.Metascore,
            ImdbRating = movie.ImdbRating
        };
    }

    #endregion
}
