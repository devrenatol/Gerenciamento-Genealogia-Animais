using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjetoLO.Domain.Entities;
using ProjetoLO.Domain.Interfaces;
using ProjetoLO.Infra.Data.Context;

namespace ProjetoLO.Infra.Data.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private ApplicationDbContext _contextAnimal;

    public AnimalRepository(ApplicationDbContext contextAnimal)
    {
        _contextAnimal = contextAnimal;
    }

    public async Task<IEnumerable<Animal>> GetAnimalsAsync()
    {
        var animais = await _contextAnimal.Animais.ToListAsync();

        if (animais.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(animais));

        return animais;
    }

    public async Task<Animal> GetByIdAsync(int id)
    {
        var animal = await _contextAnimal.Animais.FirstOrDefaultAsync(a => a.Id == id);

        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        return animal;
    }

    public async Task<IEnumerable<Animal>> GetFemeas()
    {
        var animais = await _contextAnimal.Animais.Where(a => a.Sexo == "F").ToListAsync();

        if (animais.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(animais));

        return animais;
    }

    public async Task<IEnumerable<Animal>> GetMachos()
    {
        var animais = await _contextAnimal.Animais.Where(a => a.Sexo == "M").ToListAsync();

        if (animais.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(animais));

        return animais;
    }

    public async Task<Animal> CreateAsync(Animal animal)
    {
        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        _contextAnimal.Animais.Add(animal);
        await _contextAnimal.SaveChangesAsync();

        return animal;
    }

    public async Task<Animal> UpdateAsync(Animal animal)
    {
        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        _contextAnimal.Animais.Update(animal);
        await _contextAnimal.SaveChangesAsync();

        return animal;
    }

    public async Task<Animal> DeleteAsync(Animal animal)
    {
        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        _contextAnimal.Animais.Remove(animal);
        await _contextAnimal.SaveChangesAsync();

        return animal;
    }
}