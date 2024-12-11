namespace ProjetoLO.Domain.Validations;

public class DomainExceptionValidation : Exception
{
    public DomainExceptionValidation(string error) : base(error)
    { }

    public static void When(bool hasError, string message)
    {
        if (hasError)
            throw new DomainExceptionValidation(message);
    }
}
