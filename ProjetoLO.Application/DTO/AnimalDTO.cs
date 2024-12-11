using ProjetoLO.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjetoLO.Application.DTO;

public class AnimalDTO
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(1)]
    public string? Sexo { get; set; }

    public int IdPai { get; set; }
    public int IdMae { get; set; }
}
