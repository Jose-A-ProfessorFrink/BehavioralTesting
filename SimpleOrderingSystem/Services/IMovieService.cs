using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem.Services;

public interface IMovieService
{
    Task<List<MovieSearch>> SearchMoviesAsync(string name);

    Task<Movie?> GetMovieAsync(string movieId);
}