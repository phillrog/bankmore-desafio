using AutoMapper;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Models;

namespace BankMore.Application.ContasCorrentes.AutoMapper;

public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        /// Commands Conta Corrente
        CreateMap<ContaCorrenteViewModel, CadastrarNovaContaCorrenteCommand>()
            .ConstructUsing(c => new CadastrarNovaContaCorrenteCommand(c.Nome, c.Senha, c.Cpf));
        CreateMap<ContaCorrenteViewModel, AlterarContaCorrenteCommand>()
            .ConstructUsing(c => new AlterarContaCorrenteCommand(c.Nome, c.Senha, c.SenhaAnterior, c.Cpf, c.Ativo ?? false));

        /// Commands Idempotencia
        CreateMap<IdempotenciaViewModel, CadastrarNovaIdempotenciaCommand>()
            .ConstructUsing(c => new CadastrarNovaIdempotenciaCommand(c.Id, c.IdContaCorrente, c.Resultado, c.Requisicao));
        CreateMap<CadastrarNovaIdempotenciaCommand, BankMore.Domain.ContasCorrentes.Models.Idempotencia>()
            .ConstructUsing(c => new BankMore.Domain.ContasCorrentes.Models.Idempotencia(c.Id, c.IdContaCorrente, c.Resultado, c.Requisicao));

        /// Commands Movimentação
        CreateMap<MovimentoViewModel, CadastrarNovaMovimentacaoCommand>()
            .ConstructUsing(c => new CadastrarNovaMovimentacaoCommand( c.Valor, c.TipoMovimento));
        CreateMap<CadastrarNovaMovimentacaoCommand, Movimento>()
            .ConstructUsing(c => new Movimento(
                                                c.Id,
                                                c.IdContaCorrente,
                                                c.DataMovimento,
                                                (char)c.TipoMovimento.ToString()[0],
                                                c.Valor
            ))
            .ForMember(
                dest => dest.TipoMovimento,
                opt => opt.MapFrom(src => (char) src.TipoMovimento.ToString()[0])
            )
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
    }
}
