using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pokemon.WebAPI;
using Pokemon.WebAPI.Auth;
using Pokemon.WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PokemonDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepository>(new UserRepository());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt::Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PokemonDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/login", [AllowAnonymous]
    async (HttpContext context, ITokenService tokenService, IUserRepository userRepository) =>
    {
        var userModel = new UserModel()
        {
            UserName = context.Request.Query["username"],
            Password = context.Request.Query["password"]
        };
        var userDto = userRepository.GetUser(userModel);
        if (userDto == null)
            return Results.Unauthorized();

        var token = tokenService.BuildToken(
            builder.Configuration["Jwt:Key"],
            builder.Configuration["Jwt:Issuer"],
            userDto);

        return Results.Ok(token);
    });

app.MapGet("/pokemons", [Authorize] async (IPokemonRepository repository) =>
        Results.Extensions.Xml(await repository.GetPokemonsAsync()))
    .Produces<List<Pokemon.WebAPI.Data.Pokemon>>(StatusCodes.Status200OK)
    .WithName("GetAllPokemons")
    .WithTags("Getters");

app.MapGet("/pokemons/search/name/{query}", [Authorize]async (string query, IPokemonRepository repository) =>
    await repository.GetPokemonsAsync(query) is { } pokemons
            ? Results.Extensions.Xml(pokemons)
            : Results.NotFound(Array.Empty<Pokemon.WebAPI.Data.Pokemon>())) 
    .Produces<List<Pokemon.WebAPI.Data.Pokemon>>(StatusCodes.Status200OK)
    .WithName("QueryPokemons")
    .WithTags("Getters");

app.MapGet("/pokemons/search/information/{information}",[Authorize] async (Information query, IPokemonRepository repository) =>
        await repository.GetPokemonsAsync(query) is { } pokemons
            ? Results.Ok(pokemons)
            : Results.NotFound(Array.Empty<Pokemon.WebAPI.Data.Pokemon>())) 
    .Produces<List<Pokemon.WebAPI.Data.Pokemon>>(StatusCodes.Status200OK)
    .WithName("QueryInformationPokemons")
    .WithTags("Getters");

app.MapGet("/pokemons/{id}", [Authorize] async (int id, IPokemonRepository repository) => 
    await repository.GetPokemonAsync(id) is { } pokemon ? 
        Results.Ok(pokemon) : Results.NotFound())
    .Produces<Pokemon.WebAPI.Data.Pokemon>(StatusCodes.Status200OK)
    .WithName("GetPokemon")
    .WithTags("Getters");

app.MapPost("/pokemons", [Authorize] async ([FromBody] Pokemon.WebAPI.Data.Pokemon pokemon, [FromServices] IPokemonRepository repository) =>
    {
        await repository.InsertPokemonAsync(pokemon);
        await repository.SaveAsync();
        return Results.Created($"/pokemons/{pokemon.Id}", pokemon);
    })
    .Accepts<Pokemon.WebAPI.Data.Pokemon>("application/json")
    .Produces<Pokemon.WebAPI.Data.Pokemon>(StatusCodes.Status201Created)
    .WithName("AddPokemon") 
    .WithTags("Creators");

app.MapPut("/pokemons", [Authorize] async ([FromBody] Pokemon.WebAPI.Data.Pokemon pokemon, IPokemonRepository repository) =>
    {
        await repository.UpdatePokemonAsync(pokemon);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Pokemon.WebAPI.Data.Pokemon>("application/json")
    .WithName("UpdatePokemon")
    .WithTags("Updaters");

app.MapDelete("pokemons/{id}", [Authorize] async (int id, IPokemonRepository repository) =>
    {
        await repository.DeletePokemonAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .WithName("DeletePokemon")
    .WithTags("Deleters");

app.UseHttpsRedirection();
app.Run();