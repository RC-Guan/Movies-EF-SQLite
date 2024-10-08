using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Infrastructure;
using MovieDatabase.Models;

namespace MovieDatabase.Application;

public class MovieService : IMovieService
{
    private readonly MoviesDbContext _moviesDbContext;

    public MovieService(MoviesDbContext moviesDbContext)
    {
        _moviesDbContext = moviesDbContext;
    }

    public async Task<List<Movie>> GetMovies()
    {
        return await _moviesDbContext.Movies.ToListAsync();
    }

    public async Task<Movie?> GetMovieById(int id)
    {
        return await _moviesDbContext.Movies.FindAsync(id);
    }

    public async Task<Movie> CreateMovie(Movie movie)
    {
        var validationContext = new ValidationContext(movie);
        Validator.ValidateObject(movie, validationContext, true);

        _moviesDbContext.Movies.Add(movie);
        await _moviesDbContext.SaveChangesAsync();
        return movie;
    }

    public async Task UpdateMovie(int id, Movie updateMovie)
    {
        var movie = await _moviesDbContext.Movies.FindAsync(id);
        if (movie is null) throw new KeyNotFoundException("Movie not found");

        var validationContext = new ValidationContext(updateMovie);
        Validator.ValidateObject(updateMovie, validationContext, true);

        movie.Name = updateMovie.Name;
        movie.Description = updateMovie.Description;
        movie.ReleaseDate = updateMovie.ReleaseDate;
        movie.Genre = updateMovie.Genre;

        await _moviesDbContext.SaveChangesAsync();
    }

    public async Task DeleteMovie(int id)
    {
        var movie = await _moviesDbContext.Movies.FindAsync(id);
        if (movie is null) throw new KeyNotFoundException("Movie not found");

        _moviesDbContext.Movies.Remove(movie);
        await _moviesDbContext.SaveChangesAsync();
    }
}