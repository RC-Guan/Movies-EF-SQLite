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
        _movieServiceMock.Setup(service => service.GetMovies()).ReturnsAsync(movies);

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
        _movieServiceMock.Setup(service => service.GetMovieById(1)).ReturnsAsync(movie);

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
        _movieServiceMock.Setup(service => service.CreateMovie(It.IsAny<Movie>())).ReturnsAsync(newMovie);

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
        _movieServiceMock.Setup(service => service.UpdateMovie(1, updatedMovie)).Returns(Task.CompletedTask);

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
        _movieServiceMock.Setup(service => service.DeleteMovie(1)).Returns(Task.CompletedTask);

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

    [Fact]
    public async Task GetMovieById_ShouldReturnNotFound()
    {
        // Arrange
        _movieServiceMock.Setup(service => service.GetMovieById(It.IsAny<int>())).ReturnsAsync((Movie)null!);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/movies/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidMovie = new Movie { Id = 0, Name = "", Genre = "", ReleaseDate = DateTime.MinValue };
        _movieServiceMock.Setup(service => service.CreateMovie(It.IsAny<Movie>()))
            .ThrowsAsync(new ArgumentException());

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/movie", invalidMovie);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMovie_ShouldReturnNotFound()
    {
        // Arrange
        var updatedMovie = new Movie
            { Id = 1, Name = "Updated Movie", Genre = "Action", ReleaseDate = new DateTime(2000, 1, 1) };
        _movieServiceMock.Setup(service => service.UpdateMovie(It.IsAny<int>(), It.IsAny<Movie>()))
            .ThrowsAsync(new KeyNotFoundException());

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("/movie/999", updatedMovie);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMovie_ShouldReturnNotFound()
    {
        // Arrange
        _movieServiceMock.Setup(service => service.DeleteMovie(It.IsAny<int>()))
            .ThrowsAsync(new KeyNotFoundException());

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => { services.AddScoped(_ => _movieServiceMock.Object); });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync("/movie/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}