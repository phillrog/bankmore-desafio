using AutoMapper;
using BankMore.Application.Transferencias.ViewModels;
using BankMore.Application.Transferencias.Commands;
using BankMore.Application.Transferencias.Interfaces;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;
using BankMore.Domain.Transferencias.Dtos;

namespace BankMore.Application.Transferencias.Services;

public class TransferenciaService : ITransferenciaService
{
    #region [ SERVIÇOS ]

    private readonly IMapper _mapper;
    private readonly IMediatorHandler _bus;

    #endregion

    #region [ CONSTRUTOR ]

    public TransferenciaService(
        IMapper mapper,
        IMediatorHandler bus)
    {
        _mapper = mapper;
        _bus = bus;
    }
    #endregion

    #region [ PESQUISAR ]
    
    #endregion

    #region [ CADASTRAR ]

    public async Task<Result<TransferenciaDto>> Cadastrar(RealizarTransferenciaViewModel realizarTransferenciaViewModel)
    {
        var registerCommand = _mapper.Map<RealizarTransferenciaCommand>(realizarTransferenciaViewModel);
        return await _bus.SendCommand<RealizarTransferenciaCommand, Result<TransferenciaDto>>(registerCommand);
    }
    #endregion

    #region [ DISPOSE ]
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #endregion
}
