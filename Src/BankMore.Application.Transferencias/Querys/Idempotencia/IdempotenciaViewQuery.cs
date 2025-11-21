using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.Transferencias.Querys
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
