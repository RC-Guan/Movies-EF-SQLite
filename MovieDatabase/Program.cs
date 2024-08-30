// Program.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieDatabase.Infrastructure;
using MovieDatabase.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Connect to DBContext
builder.Services.AddDbContext<MoviesDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DbConnectionString"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "MovieDatabase API",
        Description = "Record the movies you love",
        Version = "v1"
    });
});

// Register IMovieService
builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieDatabase API V1"); });
}

Query.GetMovies(app);
Query.GetMovieById(app);
Command.CreateMovie(app);
Command.UpdateMovie(app);
Command.DeleteMovie(app);

app.Run();