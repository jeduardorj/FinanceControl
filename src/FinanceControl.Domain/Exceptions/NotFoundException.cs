

namespace FinanceControl.Domain.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) 
        : base(message, 404)
    {
    }

    public NotFoundException(string entity, Guid id) 
        : base ($"{entity} com id {id} não foi encontrado.", 404)
    {
    }
}
