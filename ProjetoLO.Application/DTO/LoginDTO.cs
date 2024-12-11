using System.ComponentModel.DataAnnotations;

namespace ProjetoLO.Application.DTO;

public class LoginDTO
{
    [Required(ErrorMessage = "O email eh obrigatorio")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A senha eh obrigatoria")]
    public string? Password { get; set; }
}
