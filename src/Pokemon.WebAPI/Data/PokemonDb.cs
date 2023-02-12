using Microsoft.EntityFrameworkCore;

namespace Pokemon.WebAPI.Data;

public class PokemonDb : DbContext
{
    public PokemonDb(DbContextOptions options) : base(options)
    {
    }

    public DbSet<global::Pokemon.WebAPI.Data.Pokemon> Pokemons => Set<global::Pokemon.WebAPI.Data.Pokemon>();
}