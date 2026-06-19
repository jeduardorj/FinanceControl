

namespace FinanceControl.Domain.Exceptions;

public class ValidationException : AppException
{
    public IEnumerable<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("Um ou mais erros de validação ocorreram", 400 )
    {
        Errors = errors;
    }
}
 