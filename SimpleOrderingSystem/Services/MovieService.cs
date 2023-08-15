using SimpleOrderingSystem.Repositories;
using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem.Services;

internal class MovieService : IMovieService
{
    private readonly IMoviesRepository _moviesRepository;

    public MovieService(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    public async Task<List<MovieSearch>> SearchMoviesAsync(string name)
    {
        return await _moviesRepository.SearchMoviesAsync(name);
    }

    public async Task<Movie?> GetMovieAsync(string movieId)
    {
        return await _moviesRepository.GetMovieAsync(movieId);
    }
}
