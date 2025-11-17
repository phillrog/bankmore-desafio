using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class MovimentoExisteQuery : IRequest<bool>
    {
        public Guid Id { get; private set; }
        public MovimentoExisteQuery(Guid id)
        {
            Id = id;
        }
    }
}
