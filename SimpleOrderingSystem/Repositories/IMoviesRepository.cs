using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem.Repositories;

public interface IMoviesRepository
{
    Task<Movie?> GetMovieAsync(string id);

    Task<List<MovieSearch>> SearchMoviesAsync(string title);
}
