using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.Application;
using MovieDatabase.Infrastructure;
using MovieDatabase.Models;

namespace MovieDatabase.Tests.ApplicationTests;

public class MovieServiceTests : IDisposable
{
    private readonly MoviesDbContext _mockMoviesDbContext;

    private readonly Movie _movieInvalidName = new()
    {
        Name = "", Description = "New Description", Genre = "Drama", ReleaseDate = new DateTime(2023, 1, 1)
    };

    private readonly MovieService _movieService;

    private readonly Movie _movieValid = new()
    {
        Name = "Movie 1",
        Description = "Description 1",
        Genre = "Action",
        ReleaseDate = new DateTime(2022, 1, 1)
    };

    private readonly Movie _movieValid2 = new()
    {
        Name = "Movie 2",
        Description = "Description 2",
        Genre = "Comedy",
        ReleaseDate = new DateTime(2023, 1, 1)
    };

    public MovieServiceTests()
    {
        var options = new DbContextOptionsBuilder<MoviesDbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
        _mockMoviesDbContext = new MoviesDbContext(options);
        _movieService = new MovieService(_mockMoviesDbContext);
    }

    public void Dispose()
    {
        _mockMoviesDbContext.Database.EnsureDeleted();
        _mockMoviesDbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetMovieAsync_ShouldReturnMovie()
    {
        await _movieService.CreateMovieAsync(_movieValid);
        await _movieService.CreateMovieAsync(_movieValid2);
        var result = await _movieService.GetMoviesAsync();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Movie 1", result[0].Name);
        Assert.Equal("Description 1", result[0].Description);
        Assert.Equal("Action", result[0].Genre);
        Assert.Equal(new DateTime(2022, 1, 1), result[0].ReleaseDate);
        Assert.Equal("Movie 2", result[1].Name);
        Assert.Equal("Description 2", result[1].Description);
        Assert.Equal("Comedy", result[1].Genre);
        Assert.Equal(new DateTime(2023, 1, 1), result[1].ReleaseDate);
    }

    [Fact]
    public async Task GetMovieByIdAsync_ShouldReturnMovie()
    {
        await _movieService.CreateMovieAsync(_movieValid);
        var result = await _movieService.GetMovieByIdAsync(_movieValid.Id);
        Assert.NotNull(result);
        Assert.Equal("Movie 1", result.Name);
        Assert.Equal("Description 1", result.Description);
        Assert.Equal("Action", result.Genre);
        Assert.Equal(new DateTime(2022, 1, 1), result.ReleaseDate);
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldAddMovie()
    {
        var result = await _movieService.CreateMovieAsync(_movieValid);

        Assert.NotNull(result);
        Assert.Equal("Movie 1", result.Name);
        Assert.Equal("Description 1", result.Description);
        Assert.Equal("Action", result.Genre);
        Assert.Equal(new DateTime(2022, 1, 1), result.ReleaseDate);
        Assert.Equal(1, _mockMoviesDbContext.Movies.Count());
    }

    [Fact]
    public async Task CreateMovieAsync_ShouldFail_WhenMovieNameIsEmpty()
    {
        var exception =
            await Assert.ThrowsAsync<ValidationException>(async () =>
                await _movieService.CreateMovieAsync(_movieInvalidName));
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
        await _mockMoviesDbContext.Movies.AddAsync(_movieValid);
        await _mockMoviesDbContext.SaveChangesAsync();

        await _movieService.UpdateMovieAsync(_movieValid.Id, _movieValid2);

        var updatedMovie = await _mockMoviesDbContext.Movies.FindAsync(_movieValid.Id);
        Assert.NotNull(updatedMovie);
        Assert.Equal("Movie 2", updatedMovie.Name);
        Assert.Equal("Description 2", updatedMovie.Description);
        Assert.Equal("Comedy", updatedMovie.Genre);
        Assert.Equal(new DateTime(2023, 1, 1), updatedMovie.ReleaseDate);
    }

    [Fact]
    public async Task UpdateMovieAsync_ShouldFail_WhenMovieNotFound()
    {
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _movieService.UpdateMovieAsync(1, _movieValid));
        Assert.Equal("Movie not found", exception.Message);
    }

    [Fact]
    public async Task UpdateMovieAsync_ShouldFail_WhenMovieNameIsInvalid()
    {
        await _mockMoviesDbContext.Movies.AddAsync(_movieValid);
        await _mockMoviesDbContext.SaveChangesAsync();


        var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _movieService.UpdateMovieAsync(
            _movieValid.Id,
            _movieInvalidName));
        Assert.Equal("Name is required", exception.Message);
    }

    [Fact]
    public async Task DeleteMovieAsync_ShouldRemoveMovie()
    {
        await _mockMoviesDbContext.Movies.AddAsync(_movieValid);
        await _mockMoviesDbContext.SaveChangesAsync();

        await _movieService.DeleteMovieAsync(_movieValid.Id);

        var deletedMovie = await _mockMoviesDbContext.Movies.FindAsync(_movieValid.Id);
        Assert.Null(deletedMovie);
    }

    [Fact]
    public async Task DeleteMovieAsync_ShouldFail_WhenMovieNotFound()
    {
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _movieService.DeleteMovieAsync(1));
        Assert.Equal("Movie not found", exception.Message);
    }
}