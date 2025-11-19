
using BankMore.Domain.Common;
using BankMore.Domain.Core.Models;
using MediatR;

namespace BankMore.Application.Common.Querys
{
    public class InformacoesContaCorrenteQuery : IRequest<Result<InformacoesContaCorrenteDto>>
    {
        public string Cpf { get; private set; }
        public int Numero { get; private set; }
        public InformacoesContaCorrenteQuery(string cpf)
        {
            Cpf = cpf;
        }

        public InformacoesContaCorrenteQuery(int numero)
        {
            Numero = numero;
        }
    }
}
