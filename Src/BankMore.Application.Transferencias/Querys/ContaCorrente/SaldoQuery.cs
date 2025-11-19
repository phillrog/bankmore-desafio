using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys.ContaCorrente
{
    public class SaldoQuery: IRequest<Result<SaldoDto>>
    {
        public int NumeroConta { get; set; }
    }
}
