
using BankMore.Application.ContasCorrentes.Validations;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class CadastrarNovaMovimentacaoCommand : MovimentoCommand, IRequest<Result<MovimentacaoRelaizadaDto>>
{
    public CadastrarNovaMovimentacaoCommand(Guid idContaCorrente, decimal valor, TipoMovimento tipo)
    {
        Id = Guid.NewGuid();
        IdContaCorrente = idContaCorrente;
        Valor = valor;
        DataMovimento = DateTime.UtcNow;
        TipoMovimento = tipo; 
    }

    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaMovimentacaoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}
