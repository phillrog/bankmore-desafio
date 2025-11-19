using AutoMapper;

using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Transferencias.Models;

namespace BankMore.Application.Transferencias.AutoMapper;

public class DomainToViewModelMappingProfile : Profile
{
    public DomainToViewModelMappingProfile()
    {

        /// Idempotencia
        CreateMap<Idempotencia, IdempotenciaViewModel>().ReverseMap();

    }
}
