
using BankMore.Application.ContasCorrentes.Validations;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Enums;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class CadastrarNovaMovimentacaoCommand : MovimentoCommand, IRequest<Result<MovimentacaoRelaizadaDto>>
{
    public CadastrarNovaMovimentacaoCommand(decimal valor, TipoMovimento tipo, string descricao)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        DataMovimento = DateTime.UtcNow;
        TipoMovimento = tipo;
        Descricao = descricao;
    }

    public CadastrarNovaMovimentacaoCommand(decimal valor, TipoMovimento tipo, string numero, string descricao)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        DataMovimento = DateTime.UtcNow;
        TipoMovimento = tipo;
        NumeroConta = Convert.ToInt32(numero);
        Descricao = descricao;
    }

    public override bool IsValid()
    {
        ValidationResult = new CadastrarNovaMovimentacaoValidation().Validate(this);
        return ValidationResult.IsValid;
    }

    public bool EhDebito() => "D".Contains((char)TipoMovimento);

    public void AtualizarIdContaCorrente(Guid id) => IdContaCorrente = id;
}
