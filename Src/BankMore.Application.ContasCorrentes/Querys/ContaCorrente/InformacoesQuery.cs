using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQuery : IRequest<Result<InformacoesViewModel>>
    {
        public string Cpf { get; private set; }
        public int Numero { get; private set; }
        public InformacoesQuery(string cpf)
        {
            Cpf = cpf;
        }

        public InformacoesQuery(int numero)
        {
            Numero = numero;
        }
    }
}
