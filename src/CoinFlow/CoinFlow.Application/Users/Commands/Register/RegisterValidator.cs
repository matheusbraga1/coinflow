using FluentValidation;

namespace CoinFlow.Application.Users.Commands.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .MaximumLength(256).WithMessage("O email deve conter até no máximo 256 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória")
            .MinimumLength(8).WithMessage("A senha deve conter no mínimo 8 caracteres.")
            .MaximumLength(100).WithMessage("A senha deve conter no máximo 100 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MinimumLength(2).WithMessage("O nome deve conter no mínimo 2 caracteres")
            .MaximumLength(100).WithMessage("O nome deve conter no máximo 100 caracteres");
    }
}
