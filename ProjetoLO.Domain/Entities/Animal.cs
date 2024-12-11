using ProjetoLO.Domain.Validations;

namespace ProjetoLO.Domain.Entities;

public class Animal
{
    public int Id { get; private set; }
    public string? Nome { get; private set; }
    public string? Sexo { get; private set; }
    public int IdPai { get; private set; }
    public int IdMae { get; private set; }

    public Animal(string nome, string sexo)
    {
        Validation(nome, sexo);

    }

    public Animal(int id, string nome, string sexo)
    {
        Validation(nome, sexo);
        DomainExceptionValidation.When(id < 0, "Numero de Id invalido");
        Id = id;
    }

    public Animal(int id, string nome, string sexo, int idPai, int idMae)
    {
        Validation(nome, sexo);
        DomainExceptionValidation.When(id < 0, "Numero de Id invalido");
        DomainExceptionValidation.When(idPai < 0, "Numero de Id Invalido");
        DomainExceptionValidation.When(idMae < 0, "Numero de Id Invalido");
        Id = id;
        IdPai = idPai;
        IdMae = idMae;
    }

    public void Update(string nome, string sexo)
    {
        Validation(nome, sexo);
    }

    public void Update(string nome, string sexo, int idPai, int idMae)
    {
        Validation(nome, sexo);
        IdPai = idPai;
        IdMae = idMae;
    }


    private void Validation(string nome, string sexo)
    {
        DomainExceptionValidation.When(string.IsNullOrEmpty(nome), "O nome deve ser informado");
        DomainExceptionValidation.When(nome.Length < 3, "O nome do animal deve ter mais do que 3 caracteres");

        DomainExceptionValidation.When(sexo != "M" && sexo != "F", "O sexo do animal deve ser 'M' para Macho ou 'F' para Femea");

        Nome = nome;
        Sexo = sexo;
    }
}
