using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Application;
using MovieDatabase.Infrastructure;
using MovieDatabase.Models;
using Xunit;
using Xunit.Sdk;

namespace MovieDatabase.Tests.ApplicationTests;

public class MovieServiceTests : IDisposable
{
    private readonly MovieService _movieService;
    private readonly MoviesDbContext _mockMoviesDbContext;

    public MovieServiceTests()
    {
        var options = new DbContextOptionsBuilder<MoviesDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockMoviesDbContext = new MoviesDbContext(options);
        _movieService = new MovieService(_mockMoviesDbContext);
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldAddMovie()
    {
        var movie = new Movie
        {
            Name = "New Movie",
            Description = "New Description",
            Genre = "Drama",
            ReleaseDate = new DateTime(2023, 1, 1)
        };

        var result = await _movieService.CreateMovieAsync(movie);

        Assert.NotNull(result);
        Assert.Equal("New Movie", result.Name);
        Assert.Equal(1, _mockMoviesDbContext.Movies.Count());
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldFail_WhenMovieNameIsEmpty()
    {
        var movie = new Movie
        {
            Name = "", Description = "New Description", Genre = "Drama", ReleaseDate = new DateTime(2023, 1, 1)
        };

        var exception =
            await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.CreateMovieAsync(movie));
        Assert.Equal("Name is required", exception.Message);
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldFail_WhenMovieNameIsTooLong()
    {
        var movie = new Movie
        {
            Name = "aaaaabbbbbcccccdddddeeeee", Description = "New Description", Genre = "Drama",
            ReleaseDate = new DateTime(2023, 1, 1)
        };

        var exception =
            await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.CreateMovieAsync(movie));
        Assert.Equal("Name cannot be longer than 20 characters", exception.Message);
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldFail_WhenMovieGenreIsInvalid()
    {
        var movie = new Movie
        {
            Name = "New Movie", Description = "New Description", Genre = "", ReleaseDate = new DateTime(2023, 1, 1)
        };

        var exception =
            await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.CreateMovieAsync(movie));
        Assert.Equal("Genre is required", exception.Message);
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldFail_WhenMovieDateIsInvalid()
    {
        var movie = new Movie
        {
            Name = "New Movie", Description = "New Description", Genre = "Drama", ReleaseDate = new DateTime(1700, 1, 1)
        };

        var exception =
            await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.CreateMovieAsync(movie));
        Assert.Equal("Release date cannot be earlier than the year 1888.", exception.Message);
    }


    [Fact]
    public async Task UpdateMovieAsync_ShouldUpdateMovie()
    {
        var movie = new Movie
        {
            Name = "Old Movie",
            Description = "Old Description",
            Genre = "Action",
            ReleaseDate = new DateTime(2022, 1, 1)
        };
        await _mockMoviesDbContext.Movies.AddAsync(movie);
        await _mockMoviesDbContext.SaveChangesAsync();

        var updateMovie = new Movie
        {
            Name = "Updated Movie",
            Description = "Updated Description",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2023, 1, 1)
        };
        await _movieService.UpdateMovieAsync(movie.Id, updateMovie);

        var updatedMovie = await _mockMoviesDbContext.Movies.FindAsync(movie.Id);
        Assert.NotNull(updatedMovie);
        Assert.Equal("Updated Movie", updatedMovie.Name);
        Assert.Equal("Updated Description", updatedMovie.Description);
        Assert.Equal("Comedy", updatedMovie.Genre);
        Assert.Equal(new DateTime(2023, 1, 1), updatedMovie.ReleaseDate);
    }

    [Fact]
    public async Task UpdateMovieAsync_ShouldFail_WhenMovieNotFound()
    {
        var updateMovie = new Movie
        {
            Name = "Updated Movie",
            Description = "Updated Description",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2023, 1, 1)
        };

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _movieService.UpdateMovieAsync(1, updateMovie));
        Assert.Equal("Movie not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMovieAsync_ShouldFail_WhenMovieNameIsInvalid()
    {
        var movie = new Movie
        {
            Name = "New Movie",
            Description = "Description",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2000, 1, 1)
        };

        await _mockMoviesDbContext.Movies.AddAsync(movie);
        await _mockMoviesDbContext.SaveChangesAsync();

        var updateMovie = new Movie
        {
            Name = "",
            Description = "Description",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2000, 1, 1)
        };

        var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.UpdateMovieAsync(
            movie.Id,
            updateMovie));
        Assert.Equal("Name is required", exception.Message);
    }

    [Fact]
    public async Task DeleteMovieAsync_ShouldRemoveMovie()
    {
        var movie = new Movie
        {
            Name = "Movie to Delete",
            Description = "Description",
            Genre = "Horror",
            ReleaseDate = new DateTime(2022, 1, 1)
        };
        await _mockMoviesDbContext.Movies.AddAsync(movie);
        await _mockMoviesDbContext.SaveChangesAsync();

        await _movieService.DeleteMovieAsync(movie.Id);

        var deletedMovie = await _mockMoviesDbContext.Movies.FindAsync(movie.Id);
        Assert.Null(deletedMovie);
    }

    [Fact]
    public async Task DeleteMovieAsync_ShouldFail_WhenMovieNotFound()
    {
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _movieService.DeleteMovieAsync(1));
        Assert.Equal("Movie not found", exception.Message);
    }

    public void Dispose()
    {
        _mockMoviesDbContext.Database.EnsureDeleted();
        _mockMoviesDbContext.Dispose();
    }
}