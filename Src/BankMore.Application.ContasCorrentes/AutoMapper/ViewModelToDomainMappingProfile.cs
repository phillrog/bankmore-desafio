using AutoMapper;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.ViewModels;

namespace BankMore.Application.ContasCorrentes.AutoMapper;

public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        /// Commands
        CreateMap<ContaCorrenteViewModel, CadastrarNovaContaCorrenteCommand>()
            .ConstructUsing(c => new CadastrarNovaContaCorrenteCommand(c.Nome, c.Senha, c.Cpf));
        CreateMap<ContaCorrenteViewModel, AlterarContaCorrenteCommand>()
            .ConstructUsing(c => new AlterarContaCorrenteCommand(c.Nome, c.Senha, c.SenhaAnterior, c.Cpf, c.Ativo ?? false));
    }
}
