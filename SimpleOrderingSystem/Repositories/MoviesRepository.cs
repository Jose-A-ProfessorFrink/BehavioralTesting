using SimpleOrderingSystem.DataModels;
using SimpleOrderingSystem.Providers;
using SimpleOrderingSystem.Models;
using Microsoft.Extensions.Options;
using SimpleOrderingSystem.Extensions;


namespace SimpleOrderingSystem.Repositories;

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
            throw new SimpleOrderingSystemException(SimpleOrderingSystemErrorType.InvalidMovieIdSupplied);
        }

        return Map(apiResponse);
    }

    public async Task<List<MovieSearch>> SearchMoviesAsync(string title)
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

    private MovieSearch Map(MovieSearchResultDataModel movie)
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

    private Movie Map(GetMovieApiResponseDataModel movie)
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
