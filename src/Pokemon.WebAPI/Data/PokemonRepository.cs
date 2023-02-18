using Microsoft.EntityFrameworkCore;

namespace Pokemon.WebAPI.Data;

public class PokemonRepository : IPokemonRepository
{
    private readonly PokemonDb _context;

    public PokemonRepository(PokemonDb context)
    {
        _context = context;
    }
    
    public Task<List<Pokemon>> GetPokemonsAsync()
    {
        return _context.Pokemons.ToListAsync();
    }

    public Task<List<Pokemon>> GetPokemonsAsync(string name)
    {
        return _context.Pokemons.Where(x => x.Name.Contains(name)).ToListAsync();
    }

    public Task<List<Pokemon>> GetPokemonsAsync(Information information)
    {
        return _context.Pokemons
            .Where(x => x.Name.Contains(information.Name) && x.Description.Contains(information.Description))
            .ToListAsync();
    }

    public Task<Pokemon?> GetPokemonAsync(int pokemonId)
    {
        return _context.Pokemons.FirstOrDefaultAsync(x => x.Id == pokemonId);
    }

    public async Task InsertPokemonAsync(Pokemon pokemon)
    {
        await _context.Pokemons.AddAsync(pokemon);
    }

    public async Task UpdatePokemonAsync(Pokemon pokemon)
    {
        var pokemonFromDb = await _context.Pokemons.FindAsync(pokemon.Id);
        if (pokemonFromDb == null)
            return;

        pokemonFromDb.Name = pokemon.Name;
        pokemonFromDb.Description = pokemon.Description;
    }

    public async Task DeletePokemonAsync(int pokemonId)
    {
        var pokemonFromDb = await _context.Pokemons.FindAsync(pokemonId);
        if (pokemonFromDb == null)
            return;

        _context.Pokemons.Remove(pokemonFromDb);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}