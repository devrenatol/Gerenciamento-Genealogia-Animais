using ProjetoLO.Application.DTO;
using ProjetoLO.Application.Pagination;

namespace ProjetoLO.Application.Interfaces;

public interface IAnimalService
{
    Task<IEnumerable<AnimalDTO>> GetAllAsync();
    Task<AnimalDTO> GetAsync(int id);
    Task<Dictionary<int, AnimalDTO>> GetArvoreGenealogicaAsync(int id);
    Task<IEnumerable<AnimalDTO>> GetAllMachosAsync();
    Task<IEnumerable<AnimalDTO>> GetAllFemeasAsync();
    Task<AnimalDTO> AddAsync(AnimalDTO animalDto);
    Task<AnimalDTO> UpdateAsync(AnimalDTO animalDto);
    Task<AnimalDTO> DeleteAsync(int id);
    Task<PagedList<AnimalDTO>> GetAnimaisPaginadosAsync(AnimaisParameters animaisParams);
}