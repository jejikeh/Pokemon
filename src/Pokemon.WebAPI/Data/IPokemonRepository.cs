namespace Pokemon.WebAPI.Data;

public interface IPokemonRepository : IDisposable
{
    public Task<List<Pokemon>> GetPokemonsAsync();
    public Task<List<Pokemon>> GetPokemonsAsync(string name);
    public Task<List<Pokemon>> GetPokemonsAsync(Information information);
    public Task<Pokemon?> GetPokemonAsync(int pokemonId);
    public Task InsertPokemonAsync(Pokemon pokemon);
    public Task UpdatePokemonAsync(Pokemon pokemon);
    public Task DeletePokemonAsync(int pokemonId);
    public Task SaveAsync();
}