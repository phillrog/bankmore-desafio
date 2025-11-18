using AutoMapper;
using BankMore.Application.ContasCorrentes.Commands;
using BankMore.Application.ContasCorrentes.Interfaces;
using BankMore.Application.ContasCorrentes.Querys;
using BankMore.Application.ContasCorrentes.ViewModels;
using BankMore.Domain.ContasCorrentes.Dtos;
using BankMore.Domain.ContasCorrentes.Interfaces.Services;
using BankMore.Domain.Core.Bus;
using BankMore.Domain.Core.Models;


namespace BankMore.Application.ContasCorrentes.Services;

public class ContaCorrenteService : IContaCorrenteService
{
    #region [ SERVIÇOS ]

    private readonly IMapper _mapper;
    private readonly IMediatorHandler _bus;
    private readonly ICorrentistaService _correntistaService;
    #endregion

    #region [ CONSTRUTOR ]

    public ContaCorrenteService(
        IMapper mapper,
        IMediatorHandler bus,
        ICorrentistaService correntistaService)
    {
        _mapper = mapper;
        _bus = bus;
        _correntistaService = correntistaService;
    }
    #endregion

    #region [ PESQUISAR ]
    public async Task<Result<InformacoesViewModel>> BuscarInformcoes(string cpf)
    {
        var query = new InformacoesQuery(cpf);
        var conta = await _bus.SendCommand<InformacoesQuery, Result<InformacoesViewModel>>(query);
        return conta;
    }

    public async Task<InformacoesViewModel> BuscarPorNumero(int numero)
    {
        var query = new InformacoesQuery(numero);
        var conta = await _bus.SendCommand<InformacoesQuery, Result<InformacoesViewModel>>(query);
        return conta.Data;
    }

    public async Task<SaldoDto> Saldo(int numero)
    {
        return await _correntistaService.Saldo(numero);        
    }
    #endregion

    #region [ CADASTRAR ]

    public void Cadastrar(ContaCorrenteViewModel contaCorrenteViewModel)
    {
        var registerCommand = _mapper.Map<CadastrarNovaContaCorrenteCommand>(contaCorrenteViewModel);
        _bus.SendCommand(registerCommand);
    }
    #endregion

    #region [ ALTERAR ]

    public void Alterar(ContaCorrenteViewModel contaCorrenteViewModel)
    {
        var updateCommand = _mapper.Map<AlterarContaCorrenteCommand>(contaCorrenteViewModel);
        _bus.SendCommand(updateCommand);
    }
    #endregion

    #region [ DISPOSE ]
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    #endregion
}
