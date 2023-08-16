using SimpleOrderingSystem.Domain.Models;

namespace SimpleOrderingSystem.Domain.Repositories;

public interface IMoviesRepository
{
    Task<Movie?> GetMovieAsync(string id);

    Task<List<MovieSummary>> SearchMoviesAsync(string title);
}
