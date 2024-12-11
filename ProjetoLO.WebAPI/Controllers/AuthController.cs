using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoLO.Application.DTO;
using ProjetoLO.Application.Interfaces;
using ProjetoLO.Infra.Data.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjetoLO.WebAPI.Controllers;

[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUsers> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUsers> userManager, RoleManager<IdentityRole> roleManager,
        ILogger<AuthController> logger, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, login.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out var refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        _logger.LogWarning("Usuario nao autorizado");
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Register([FromBody] RegisterDTO register)
    {
        var userExists = await _userManager.FindByEmailAsync(register.Email!);

        if (userExists is not null)
        {
            _logger.LogWarning("Usuario ja existe");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { StatusCode = "Erro", Message = "Usuario ja existe" });
        }

        var user = new ApplicationUsers
        {
            Email = register.Email,
            UserName = register.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, register.Password!);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Erro ao criar usuario");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { StatusCode = "Erro", Message = "Erro ao criar usuario" });
        }

        return Ok(new ResponseDTO { StatusCode = "Sucesso", Message = "Usuario criado com sucesso!" });
    }

    [HttpPost]
    [Route("refresh-token")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDTO token)
    {
        if (token is null)
        {
            _logger.LogWarning("Usuario invalido para operacao");
            return BadRequest("Usuario invalido para operacao");
        }

        string? accessToken = token.AccessToken ?? throw new ArgumentNullException(nameof(token));
        string? refreshToken = token.RefreshToken ?? throw new ArgumentNullException(nameof(token));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, _configuration);

        if (principal is null)
        {
            _logger.LogWarning("Token/RefreshToken invalidos");
            return BadRequest("Token/RefreshToken invalidos");
        }

        string userName = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(userName!);

        if (user is null || refreshToken != user.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            _logger.LogWarning("Token/RefreshToken invalidos");
            return BadRequest("Token/RefreshToken invalidos");
        }

        var novoAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
        var novoRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = novoRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(novoAccessToken),
            refreshToken = novoRefreshToken,
        });
    }

    [HttpPost]
    [Route("CreateRole")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Role criada");
                return StatusCode(StatusCodes.Status201Created,
                        new ResponseDTO { StatusCode = "Sucesso", Message = $"Role {roleName} adicionada com sucesso!!" });
            }
            else
            {
                _logger.LogInformation(1, "Error");
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseDTO { StatusCode = "Erro", Message = $"Erro ao criar a role {roleName}" });
            }
        }

        _logger.LogWarning("A role ja existe");
        return StatusCode(StatusCodes.Status400BadRequest,
                        new ResponseDTO { StatusCode = "Erro", Message = $"A role {roleName} ja existe" });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"Usuario {user.Email} adicionado a role {roleName}!");
                return StatusCode(StatusCodes.Status200OK,
                    new ResponseDTO { StatusCode = "Sucesso", Message = $"Usuario {user.Email} adicionado a role {roleName}!" });
            }
            else
            {
                _logger.LogInformation(2, $"Erro: Erro ao adicionar o usuario {user.Email} a role {roleName}");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseDTO { StatusCode = "Erro", Message = $"Erro: Erro ao adicionar o usuario {user.Email} a role {roleName}" });
            }
        }

        return BadRequest(new { Error = "Nao foi possivel encontrar o usuario" });
    }
}
