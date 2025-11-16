using AutoMapper;

using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Application.ContasCorrentes.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {
        CreateMap<ContaCorrente, ContaCorrenteViewModel>();
        CreateMap<ContaCorrente, InformacoesViewModel>();
    }
}
