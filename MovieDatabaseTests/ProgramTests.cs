using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MovieDatabase.Application;
using MovieDatabase.Models;

namespace MovieDatabase.Tests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMovieService> _movieServiceMock;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _movieServiceMock = new Mock<IMovieService>();
    }

    [Fact]
    public async Task GetMovies_ShouldReturnAllMovies()
    {
        // Arrange
        var movies = new List<Movie>
        {
            new() { Id = 1, Name = "Movie 1", Genre = "Action", ReleaseDate = new DateTime(2000, 1, 1) },
            new() { Id = 2, Name = "Movie 2", Genre = "Comedy", ReleaseDate = new DateTime(2010, 1, 1) }
        };
        _movieServiceMock.Setup(service => service.GetMoviesAsync()).ReturnsAsync(movies);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/movies");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<Movie>>();

        // Assert
        Assert.Equal(2, result!.Count);
        Assert.Contains(result, m => m.Name == "Movie 1");
        Assert.Contains(result, m => m.Name == "Movie 2");
    }

    [Fact]
    public async Task GetMovieById_ShouldReturnCorrectMovie()
    {
        // Arrange
        var movie = new Movie { Id = 1, Name = "Movie 1", Genre = "Action", ReleaseDate = new DateTime(2000, 1, 1) };
        _movieServiceMock.Setup(service => service.GetMovieByIdAsync(1)).ReturnsAsync(movie);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/movies/1");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Movie>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Movie 1", result.Name);
    }

    [Fact]
    public async Task CreateMovie_ShouldAddMovie()
    {
        // Arrange
        var newMovie = new Movie { Id = 3, Name = "Movie 3", Genre = "Drama", ReleaseDate = new DateTime(2020, 1, 1) };
        _movieServiceMock.Setup(service => service.CreateMovieAsync(It.IsAny<Movie>())).ReturnsAsync(newMovie);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/movie", newMovie);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Movie>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Movie 3", result.Name);
    }

    [Fact]
    public async Task UpdateMovie_ShouldModifyMovie()
    {
        // Arrange
        var updatedMovie = new Movie
            { Id = 1, Name = "Updated Movie", Genre = "Action", ReleaseDate = new DateTime(2000, 1, 1) };
        _movieServiceMock.Setup(service => service.UpdateMovieAsync(1, updatedMovie)).Returns(Task.CompletedTask);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("/movie/1", updatedMovie);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_ShouldRemoveMovie()
    {
        // Arrange
        _movieServiceMock.Setup(service => service.DeleteMovieAsync(1)).Returns(Task.CompletedTask);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync("/movie/1");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}