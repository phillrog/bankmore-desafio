using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.Common.Querys.ContaCorrente
{
    public class SaldoQuery : IRequest<Result<SaldoDto>>
    {
        public int NumeroConta { get; set; }
    }
}
