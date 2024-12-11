using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoLO.Application.DTO;

public class RegisterDTO
{
    [Required(ErrorMessage = "O email eh obrigatorio")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O username eh obrigatorio")]
    [MaxLength(100)]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "A senha eh obrigatoria")]
    [PasswordPropertyText]
    [MinLength(5)]
    public string? Password { get; set; }
}
