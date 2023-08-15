using Microsoft.AspNetCore.Mvc;
using SimpleOrderingSystem.Services;
using SimpleOrderingSystem.ViewModels;
using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem.Controllers;

[Route("[controller]")]
[ApiController]
public class MoviesController
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Returns a movie by its movie id (Imdb id)
    /// </summary>
    /// <param name="movieId"></param>
    /// <returns></returns>
    [Route("{movieId}")]
    [HttpGet]
    [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieViewModel>> GetMovie(string movieId)
    {
        var result = await _movieService.GetMovieAsync(movieId);

        if (movieId is null)
        {
            return new NotFoundResult();
        }

        return Map(result);
    }

    /// <summary>
    /// Searches for movies based on their title. Returns a maximum of 5 results.
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Route("search")]
    [HttpGet]
    [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieSearchResponseViewModel>> SearchCustomer([FromQuery] MovieSearchRequestViewModel searchRequest)
    {
        var movies = await _movieService.SearchMoviesAsync(searchRequest.Name!);

        return new MovieSearchResponseViewModel
        {
            Movies = movies.Select(a=> Map(a)).ToList()
        };
    }

    #region Helpers

    private MovieSearchViewModel Map(MovieSearch movie)
    {
        return new()
        {
            Id = movie.Id,
            Type = movie.Type,
            Title = movie.Title,
            Year = movie.Year,
            PosterUrl = movie.PosterUrl
        };
    }

    private MovieViewModel Map(Movie movie)
    {
        return new()
        {
            Id = movie.Id,
            Type = movie.Type,
            Title = movie.Title,
            Year = movie.Year,
            Rating = movie.Rating,
            PlotDescription = movie.PlotDescription,
            DateReleased = movie.DateReleased,
            PosterUrl = movie.PosterUrl,
            Metascore = movie.Metascore,
            ImdbRating = movie.ImdbRating
        };
    }

    #endregion
}

