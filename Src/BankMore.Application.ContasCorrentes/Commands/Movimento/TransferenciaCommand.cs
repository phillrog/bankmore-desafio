using BankMore.Domain.ContasCorrentes.Enums;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Commands;

public class TransferenciaCommand : IRequest<bool>
{
    public Guid IdContaCorrente { get; set; }
    public DateTime DataMovimento { get; set; }
    public TipoMovimento TipoMovimento { get; set; }
    public decimal Valor { get; set; }

    public TransferenciaCommand(decimal valor, TipoMovimento tipo, Guid idContaCorrente)
    {
        Valor = valor;
        TipoMovimento = tipo;
        IdContaCorrente = idContaCorrente;
    }
}