using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class IdempotenciaExisteQuery : IRequest<bool>
    {
        public Guid Id { get; private set; }
        public IdempotenciaExisteQuery(Guid id)
        {
            Id = id;
        }
    }
}
