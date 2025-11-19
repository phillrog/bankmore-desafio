using AutoMapper;
using BankMore.Application.Transferencia.Commands.Transferencia;
using BankMore.Application.Transferencia.ViewModels;
using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Domain.Transferencias.Models;

namespace BankMore.Application.Transferencias.AutoMapper;

public class ViewModelToDomainMappingProfile : Profile
{
    public ViewModelToDomainMappingProfile()
    {
        /// Commands Conta Corrente
        CreateMap<RealizarTransferenciaViewModel, TransferenciaCommand>()
            .ConstructUsing(c => new RealizarTransferenciaCommand(c.NumeroContaCorrenteOrigem, c.NumneroContaCorrenteDestino, c.Valor));
                
        /// Commands Idempotencia
        CreateMap<IdempotenciaViewModel, CadastrarNovaIdempotenciaCommand>()
            .ConstructUsing(c => new CadastrarNovaIdempotenciaCommand(c.Id, c.IdContaCorrente, c.Resultado, c.Requisicao));
        CreateMap<CadastrarNovaIdempotenciaCommand, Idempotencia>()
            .ConstructUsing(c => new Idempotencia(c.Id, c.IdContaCorrente, c.Resultado, c.Requisicao));
    }
}
