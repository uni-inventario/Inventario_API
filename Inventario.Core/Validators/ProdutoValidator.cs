using FluentValidation;
using Inventario.Core.Models;

namespace Inventario.Core.Validators
{
    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {
            RuleFor(Usuario => Usuario.Nome)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .Length(2, 250).WithMessage("O nome do produto deve ter entre 2 e 250 caracteres.");

            RuleFor(Usuario => Usuario.Descricao)
                .NotEmpty().WithMessage("A descrição do produto é obrigatório.")
                .Length(2, 1000).WithMessage("A descrição do produto deve ter entre 2 e 1000 caracteres.");

            RuleFor(Usuario => Usuario.Preco)
                .NotEmpty().WithMessage("O preço do produto é obrigatório.")
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");
                
            RuleFor(Usuario => Usuario.Quantidade)
                .NotEmpty().WithMessage("A quantidade do produto é obrigatória.")
                .GreaterThanOrEqualTo(0).WithMessage("A quantidade do produto não pode ser negativa.");
        }
    }
}