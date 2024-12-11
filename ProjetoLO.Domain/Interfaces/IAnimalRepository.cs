using ProjetoLO.Domain.Entities;

namespace ProjetoLO.Domain.Interfaces;

public interface IAnimalRepository
{
    Task<IEnumerable<Animal>> GetAnimalsAsync();
    Task<Animal> GetByIdAsync(int id);
    Task<IEnumerable<Animal>> GetFemeas();
    Task<IEnumerable<Animal>> GetMachos();
    Task<Animal> CreateAsync(Animal animal);
    Task<Animal> UpdateAsync(Animal animal);
    Task<Animal> DeleteAsync(Animal animal);
}
