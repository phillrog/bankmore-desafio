using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class MovimentoViewQuery : IRequest<Result<MovimentoViewModel>>
    {
        public MovimentoViewQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
