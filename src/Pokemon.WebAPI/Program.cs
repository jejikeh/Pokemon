using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokemon.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PokemonDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PokemonDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/pokemons", async (IPokemonRepository repository) => 
    Results.Ok(await repository.GetPokemonsAsync()));

app.MapGet("/pokemons/{id}", async (int id, IPokemonRepository repository) => 
    await repository.GetPokemonAsync(id) is { } pokemon ? 
        Results.Ok(pokemon) : Results.NotFound());

app.MapPost("/pokemons", async ([FromBody] Pokemon.WebAPI.Data.Pokemon pokemon, [FromServices] IPokemonRepository repository) =>
{
    await repository.InsertPokemonAsync(pokemon);
    await repository.SaveAsync();
    return Results.Created($"/pokemons/{pokemon.Id}", pokemon);
});

app.MapPut("/pokemons", async ([FromBody] Pokemon.WebAPI.Data.Pokemon pokemon, IPokemonRepository repository) =>
{
    await repository.UpdatePokemonAsync(pokemon);
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("pokemons/{id}", async (int id, IPokemonRepository repository) =>
{
    await repository.DeletePokemonAsync(id);
    await repository.SaveAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();
app.Run();