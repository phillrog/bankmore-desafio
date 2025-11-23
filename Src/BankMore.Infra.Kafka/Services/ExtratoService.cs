using BankMore.Domain.Common.Dtos;
using BankMore.Domain.Common.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace BankMore.Infra.Kafka.Services
{
    #region [ Views ]


    internal class Movimentacao
    {
        public Guid Id { get; set; }
        public Guid IdTransferencia { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; }
        public decimal Valor { get; set; }
        public Guid IdContaCorrente { get; set; } // Adicionado para clareza
    }


    #endregion


    public class ExtratoService : IExtratoService
    {
        private readonly string _contaCorrenteConnectionString; // Banco A
        private readonly string _transferenciaConnectionString; // Banco B

        public ExtratoService(IConfiguration configuration)
        {
            _contaCorrenteConnectionString = configuration.GetConnectionString("DefaultConnection");
            _transferenciaConnectionString = configuration.GetConnectionString("TransferenciaDb");
        }

        public async Task<IEnumerable<ExtratoDto>> Gerar(int numeroConta)
        {
            Dictionary<Guid, dynamic> transferenciasMap = new();
            Dictionary<Guid, dynamic> contasDestinoMap = new();
            Guid idContaOrigem = Guid.Empty;
            List<Guid> idsContasDestino = new();
            List<Movimentacao> movimentacoes = new();

            #region [ BANO CONTA CORRENTE ]


            using (var contaConexao = new SqlConnection(_contaCorrenteConnectionString))
            {
                await contaConexao.OpenAsync();

                // 1a. Buscar o ID da Conta
                #region [ BUSCA CONTA ]

                var contaSql = "SELECT id FROM contacorrente WHERE numero = @NumeroConta AND IsDeleted = 0;";
                idContaOrigem = await contaConexao.QuerySingleOrDefaultAsync<Guid>(
                    contaSql,
                    new { NumeroConta = numeroConta }
                );
                #endregion

                if (idContaOrigem == Guid.Empty) return Enumerable.Empty<ExtratoDto>();

                #region [ BUSCA MOVIMENTAÇÕES ]

                var movimentosSql = @"
                SELECT id, idtransferencia, datamovimento, tipomovimento, valor, idcontacorrente
                FROM movimento
                WHERE idcontacorrente = @IdConta AND IsDeleted = 0
                ORDER BY datamovimento DESC;";

                movimentacoes = (await contaConexao.QueryAsync<Movimentacao>(
                    movimentosSql,
                    new { IdConta = idContaOrigem }
                )).ToList();
                #endregion
            }

            if (!movimentacoes.Any()) return Enumerable.Empty<ExtratoDto>();

            #region [ PEGA AS TRANSFERENCIAS ]

            var idsTransferencia = movimentacoes
                .Where(m => m.IdTransferencia != Guid.Empty)
                .Select(m => m.IdTransferencia)
                .Distinct()
                .ToList();

            // Se houver transferências para buscar detalhes
            if (idsTransferencia.Any())
            {
                using (var transfConexao = new SqlConnection(_transferenciaConnectionString))
                {
                    await transfConexao.OpenAsync();

                    var transferenciasSql = @"
                    SELECT id, idcontacorrenteorigem, idcontacorrentedestino
                    FROM transferencia
                    WHERE id IN @Ids AND IsDeleted = 0;";

                    var detalhes = await transfConexao.QueryAsync(
                        transferenciasSql,
                        new { Ids = idsTransferencia }
                    );

                    foreach (var t in detalhes)
                    {
                        var idTransferencia = (Guid)t.id;
                        transferenciasMap.Add(idTransferencia, t);

                        var idDestino = (Guid)t.idcontacorrentedestino;
                        idsContasDestino.Add(idDestino);
                    }
                }
            }
            #endregion
            #endregion

            #region [ BANCO TRANSFERENCIAS ]


            var distinctIdsContasDestino = idsContasDestino.Distinct().ToList();

            #region [ CONTAS CREDITADAS ]

            if (distinctIdsContasDestino.Any())
            {
                using (var contaConexao = new SqlConnection(_contaCorrenteConnectionString))
                {
                    await contaConexao.OpenAsync();

                    var contasSql = "SELECT id, nome, numero FROM contacorrente WHERE id IN @Ids AND IsDeleted = 0;";

                    var detalhesContas = await contaConexao.QueryAsync(
                        contasSql,
                        new { Ids = distinctIdsContasDestino }
                    );

                    contasDestinoMap = detalhesContas.ToDictionary(c => (Guid)c.id, c => c);
                }
            }
            #endregion

            #region [ CHEGA FIM ]

            var extratoFinal = new List<ExtratoDto>();

            int contaAtualNumero = numeroConta;

            foreach (var movimentacao in movimentacoes)
            {
                string nomeContraparte = "MOVIMENTAÇÃO INTERNA";
                int numeroContraparte = 0;

                int numeroOrigem = contaAtualNumero;
                int numeroDestino = 0;

                if (transferenciasMap.TryGetValue(movimentacao.IdTransferencia, out dynamic transferencia))
                {
                    var idContraparte = movimentacao.TipoMovimento == 'D'
                                         ? (Guid)transferencia.idcontacorrentedestino
                                         : (Guid)transferencia.idcontacorrenteorigem;

                    if (contasDestinoMap.TryGetValue(idContraparte, out dynamic contaDetalhe))
                    {
                        nomeContraparte = contaDetalhe.nome;
                        numeroContraparte = contaDetalhe.numero;
                    }

                    if (movimentacao.TipoMovimento == 'D')
                    {
                        numeroDestino = numeroContraparte;
                        numeroOrigem = contaAtualNumero;
                    }
                    else
                    {
                        numeroDestino = contaAtualNumero;
                        numeroOrigem = numeroContraparte;
                    }
                }

                extratoFinal.Add(new ExtratoDto
                {
                    Id = movimentacao.Id,
                    Data = movimentacao.DataMovimento,
                    Valor = movimentacao.Valor,
                    Tipo = movimentacao.TipoMovimento == 'C' ? "CRÉDITO" : "DÉBITO",
                    NumeroContaOrigem = numeroOrigem,
                    NomeContraparte = nomeContraparte,
                    NumeroContaDestino = numeroDestino
                });
            }
            #endregion

            #endregion

            return extratoFinal
            .Distinct()
            .OrderByDescending(e => e.Data)
            .ToList();
        }
    }
}
