using AutoMapper;

using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Application.ContasCorrentes.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        /// Conta Corrente
        CreateMap<ContaCorrente, NovaCorrenteViewModel>();
        CreateMap<ContaCorrente, ContaCorrenteViewModel>();
        CreateMap<ContaCorrente, InformacoesViewModel>();

        /// Idempotencia
        CreateMap<BankMore.Domain.ContasCorrentes.Models.Idempotencia, IdempotenciaViewModel>().ReverseMap();

        /// Movimentações
        CreateMap<Movimento, MovimentoViewModel>().ReverseMap();
    }
}
