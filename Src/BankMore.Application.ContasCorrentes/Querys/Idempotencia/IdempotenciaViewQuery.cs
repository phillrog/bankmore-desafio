using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class IdempotenciaViewQuery : IRequest<Result<IdempotenciaViewModel>>
    {
        public IdempotenciaViewQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
