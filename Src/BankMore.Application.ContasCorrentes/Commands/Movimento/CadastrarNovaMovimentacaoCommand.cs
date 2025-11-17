
using BankMore.Application.ContasCorrentes.Validations;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class CadastrarNovaMovimentacaoCommand : MovimentoCommand, IRequest<Result<MovimentacaoRelaizadaDto>>
{
    public CadastrarNovaMovimentacaoCommand(decimal valor, TipoMovimento tipo)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        DataMovimento = DateTime.UtcNow;
        TipoMovimento = tipo; 
    }

    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaMovimentacaoValidation().Validate(this);
        return ValidationResult.IsValid;
    }

    public bool EhDebito() => "D".Contains((char)TipoMovimento);

    public void AtualizarIdContaCorrente(Guid id) => IdContaCorrente = id;
}
