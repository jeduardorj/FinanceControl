using FluentValidation;
using FinanceControl.Application.DTOs.Transaction;

namespace FinanceControl.Application.Validators;

public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(200).WithMessage("A descrição deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("A data é obrigatória.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("A data não pode ser futura.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo deve ser Income ou Expense.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");
    }
}