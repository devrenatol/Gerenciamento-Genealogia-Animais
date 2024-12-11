using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ProjetoLO.Application.DTO;
using ProjetoLO.Application.Interfaces;
using ProjetoLO.Application.Pagination;
using ProjetoLO.Domain.Entities;
using ProjetoLO.Domain.Interfaces;

namespace ProjetoLO.Application.Services;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _repository;
    private readonly IMapper _mapper;

    public AnimalService(IAnimalRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AnimalDTO>> GetAllAsync()
    {
        var animais = await _repository.GetAnimalsAsync();

        if (animais is null)
            throw new ArgumentNullException();

        var animaisDto = _mapper.Map<IEnumerable<AnimalDTO>>(animais);

        return animaisDto;
    }

    public async Task<Dictionary<int, AnimalDTO>> GetArvoreGenealogicaAsync(int id)
    {
        var animal = await _repository.GetByIdAsync(id);

        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        return await ConstruirArvoreGenealogica(animal);
    }

    public async Task<AnimalDTO> GetAsync(int id)
    {
        var animal = await _repository.GetByIdAsync(id);

        if (animal is null)
            throw new ArgumentNullException(nameof(animal));

        var animalDto = _mapper.Map<AnimalDTO>(animal);

        return animalDto!;
    }

    public async Task<PagedList<AnimalDTO>> GetAnimaisPaginadosAsync(AnimaisParameters animaisParams)
    {
        var animais = await _repository.GetAnimalsAsync();

        if (animais is null)
            throw new ArgumentNullException();

        var animaisDto = _mapper.Map<IEnumerable<AnimalDTO>>(animais);

        var animaisPaginados = PagedList<AnimalDTO>.ToPagedList(animaisDto.AsQueryable(), animaisParams.PageNumber, animaisParams.PageSize);

        return animaisPaginados;
    }

    public async Task<IEnumerable<AnimalDTO>> GetAllMachosAsync()
    {
        var animais = await _repository.GetMachos();

        if (animais.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(animais));

        var animaisDto = _mapper.Map<IEnumerable<AnimalDTO>>(animais);

        return animaisDto;
    }

    public async Task<IEnumerable<AnimalDTO>> GetAllFemeasAsync()
    {
        var animais = await _repository.GetFemeas();

        if (animais.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(animais));

        var animaisDto = _mapper.Map<IEnumerable<AnimalDTO>>(animais);

        return animaisDto;
    }

    public async Task<AnimalDTO> AddAsync(AnimalDTO animalDto)
    {
        if (animalDto is null)
            throw new ArgumentNullException();

        var animal = _mapper.Map<Animal>(animalDto);
        var animalCriado = await _repository.CreateAsync(animal!);
        var animalCriadoDto = _mapper.Map<AnimalDTO>(animalCriado);

        return animalCriadoDto!;
    }

    public async Task<AnimalDTO> UpdateAsync(AnimalDTO animalDto)
    {
        if (animalDto is null)
            throw new ArgumentNullException();

        var animal = _mapper.Map<Animal>(animalDto);
        var animalModificado = await _repository.UpdateAsync(animal!);
        var animalModificadoDto = _mapper.Map<AnimalDTO>(animalModificado);

        return animalModificadoDto!;
    }

    public async Task<AnimalDTO> DeleteAsync(int id)
    {
        var animalDto = _repository.GetByIdAsync(id).Result;

        if (animalDto is null)
            throw new ArgumentNullException();

        var animalExcluido = await _repository.DeleteAsync(animalDto);
        var animalExcluidoDto = _mapper.Map<AnimalDTO>(animalExcluido);

        return animalExcluidoDto!;
    }

    private async Task<Dictionary<int, AnimalDTO>> ConstruirArvoreGenealogica(Animal animal)
    {
        if (animal == null)
            throw new ArgumentNullException(nameof(animal));

        var arvoreGenealogica = new Dictionary<int, AnimalDTO>();
        var fila = new Queue<int>();

        fila.Enqueue(animal.Id);

        while (fila.Count > 0)
        {
            var idAtual = fila.Dequeue();

            if (arvoreGenealogica.ContainsKey(idAtual))
                continue;

            var animalAtual = await _repository.GetByIdAsync(idAtual);

            if (animalAtual == null)
                continue;

            var animaAtualDTO = _mapper.Map<AnimalDTO>(animalAtual);

            arvoreGenealogica[idAtual] = animaAtualDTO;

            if (animalAtual.IdPai != 0)
                fila.Enqueue(animalAtual.IdPai);

            if (animalAtual.IdMae != 0)
                fila.Enqueue(animalAtual.IdMae);
        }

        return arvoreGenealogica;
    }
}
