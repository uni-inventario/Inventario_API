using FluentValidation;
using Inventario.Core.Models;

namespace Inventario.Core.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(Usuario => Usuario.Nome)
                .NotEmpty().WithMessage("O nome do Usuario é obrigatório.")
                .Length(2, 250).WithMessage("O nome do Usuario deve ter entre 2 e 100 caracteres.");

            RuleFor(Usuario => Usuario.Email)
                .NotEmpty().WithMessage("O email do Usuario é obrigatório.")
                .EmailAddress().WithMessage("O email do Usuario deve ser um endereço de email válido.")
                .Length(5, 250).WithMessage("O email do Usuario deve ter entre 5 e 250 caracteres.");

        }
    }
}