using BankMore.Application.ContasCorrentes.ViewModels;
using MediatR;

namespace BankMore.Application.ContasCorrentes.Querys
{
    public class InformacoesQuery : IRequest<InformacoesViewModel>
    {
        public string Cpf { get; set; }
        public InformacoesQuery(string cpf)
        {
            Cpf = cpf;
        }
    }
}
