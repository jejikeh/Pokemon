using Microsoft.EntityFrameworkCore;

namespace Pokemon.WebAPI.Data;

public class PokemonDb : DbContext
{
    public PokemonDb(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Pokemon> Pokemons => Set<Pokemon>();
}