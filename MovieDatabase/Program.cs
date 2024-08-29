using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieDatabase.Infrastructure;
using MovieDatabase.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieDatabase API V1");
    });
}

app.MapGet("/movies", async (MoviesDbContext db) => await db.Movies.ToListAsync());

app.MapGet("/movies/{id}", async (MoviesDbContext db, int id) => await db.Movies.FindAsync(id));

app.MapPost("/movie", async (MoviesDbContext db, Movie movie) =>
{
    await db.Movies.AddAsync(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/movie/{movie.Id}", movie);
});

app.MapPut("/movie/{id}", async (MoviesDbContext db, Movie updateMovie, int id) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();
    movie.Name = updateMovie.Name;
    movie.Description = updateMovie.Description;
    movie.ReleaseDate = updateMovie.ReleaseDate;
    movie.Genre = updateMovie.Genre;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/movie/{id}", async (MoviesDbContext db, int id) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null)
    {
        return Results.NotFound();
    }

    db.Movies.Remove(movie);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
