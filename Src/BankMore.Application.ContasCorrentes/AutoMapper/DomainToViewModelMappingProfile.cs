using AutoMapper;

using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.Common;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Application.ContasCorrentes.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        /// Conta Corrente
        CreateMap<ContaCorrente, NovaCorrenteViewModel>();
        CreateMap<ContaCorrente, ContaCorrenteViewModel>();
        CreateMap<ContaCorrente, InformacoesContaCorrenteDto>();

        /// Idempotencia
        CreateMap<BankMore.Domain.ContasCorrentes.Models.Idempotencia, IdempotenciaViewModel>().ReverseMap();

        /// Movimentações
        CreateMap<Movimento, MovimentoViewModel>().ReverseMap();
    }
}
