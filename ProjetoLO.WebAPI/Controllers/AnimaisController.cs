using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProjetoLO.Application.DTO;
using ProjetoLO.Application.Interfaces;
using ProjetoLO.Application.Pagination;
using Microsoft.AspNetCore.Http;

namespace ProjetoLO.WebAPI.Controllers;

[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class AnimaisController : ControllerBase
{
    private readonly IAnimalService _service;
    private readonly ILogger<AnimaisController> _logger;

    public AnimaisController(IAnimalService service, ILogger<AnimaisController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimais()
    {
        var animaisDto = await _service.GetAllAsync();

        if (animaisDto is null)
        {
            _logger.LogWarning("Animais nao encontrados");
            return NotFound("Animais nao encontrados");
        }

        return Ok(animaisDto);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AnimalDTO>> GetAnimalId(int id)
    {
        var animalDto = await _service.GetAsync(id);

        if (animalDto is null)
        {
            _logger.LogWarning("Animal nao encontrados");
            return NotFound("Animal nao encontrados");
        }

        return Ok(animalDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimais([FromQuery] AnimaisParameters parameters)
    {
        var animais = await _service.GetAnimaisPaginadosAsync(parameters);

        return ObterAnimaisPaginados(animais);
    }

    [HttpGet("arvore/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Dictionary<int, AnimalDTO>>> GetArvore(int id)
    {
        var animalDto = await _service.GetArvoreGenealogicaAsync(id);

        if (animalDto is null)
        {
            _logger.LogWarning("Animais nao encontrados");
            return NotFound("Animais nao encontrados");
        }

        return Ok(animalDto);
    }

    [HttpGet("machos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAllMachos()
    {
        var animaisDto = await _service.GetAllMachosAsync();

        if (animaisDto.IsNullOrEmpty())
        {
            _logger.LogWarning("Animais nao encontrados");
            return NotFound("Animais nao encontrados");
        }

        return Ok(animaisDto);
    }

    [HttpGet("femeas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAllFemeas()
    {
        var animaisDto = await _service.GetAllFemeasAsync();

        if (animaisDto.IsNullOrEmpty())
        {
            _logger.LogWarning("Animais nao encontrados");
            return NotFound("Animais nao encontrados");
        }

        return Ok(animaisDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AnimalDTO>> Post(AnimalDTO animalDto)
    {
        if (animalDto is null)
        {
            _logger.LogWarning("Dados invalidos");
            return BadRequest("Dados invalidos");
        }

        var animalCriadoDto = await _service.AddAsync(animalDto);

        return Ok(animalCriadoDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AnimalDTO>> Put(int id, AnimalDTO animalDto)
    {
        if (animalDto is null)
        {
            _logger.LogWarning("Dados invalidos");
            return BadRequest("Dados invalidos");
        }

        var animalModificadoDto = await _service.UpdateAsync(animalDto);

        return Ok(animalModificadoDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AnimalDTO>> Delete(int id)
    {
        var animalDto = await _service.GetAsync(id);

        if (animalDto is null)
        {
            _logger.LogWarning($"Animal com id = {id}. Nao encontrado!");
            return NotFound($"Animal com id = {id}. Nao encontrado!");
        }

        var animalDeletado = await _service.DeleteAsync(id);

        return Ok(animalDeletado);
    }

    private ActionResult<IEnumerable<AnimalDTO>> ObterAnimaisPaginados(PagedList<AnimalDTO> animais)
    {
        var metadata = new
        {
            animais.TotalCount,
            animais.PageSize,
            animais.CurrentPage,
            animais.TotalPages,
            animais.HasPrevious,
            animais.HasNext
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var animaisDto = animais.ToList();

        return Ok(animaisDto);
    }
}
