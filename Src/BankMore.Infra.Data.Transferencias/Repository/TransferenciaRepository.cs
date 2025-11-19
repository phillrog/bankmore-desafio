using BankMore.Domain.Transferencias.Interfaces;
using BankMore.Domain.Transferencias.Models;
using BankMore.Infra.Data.Common.Repository;
using BankMore.Infra.Data.Transferencias.Context;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankMore.Infra.Data.Transferencias.Repository;

public class TransferenciaRepository : Repository<Transferencia, ApplicationDbContext>, ITransferenciaRepository
{
    public TransferenciaRepository(ApplicationDbContext context) : base(context)
    {
    }

    //public async Task<SaldoDto> BuscarSaldoPorNumeroAsync(int numeroConta)
    //{
    //    #region [ SQL ]

    //    var sql = @"
    //        SELECT
    //                -- Total de Créditos
    //                ISNULL(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) AS TotalCredito,

    //                -- Total de Débitos
    //                ISNULL(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) AS TotalDebito,

    //                -- Saldo Atual (Crédito - Débito)
    //                ISNULL(SUM(CASE 
    //                    WHEN tipomovimento = 'C' THEN valor 
    //                    WHEN tipomovimento = 'D' THEN valor * -1
    //                    ELSE 0
    //                END), 0) AS SaldoAtualizado
    //           FROM movimento M
    //      LEFT JOIN CONTACORRENTE C ON M.idcontacorrente = C.Id
    //          WHERE C.numero = @NumeroConta
    //       GROUP BY C.ID";
    //    #endregion

    //    using (IDbConnection conexao = new SqlConnection(_db.Database.GetConnectionString()))
    //    {

    //        var saldo = await conexao.QueryFirstOrDefaultAsync<SaldoDto>(
    //            sql,
    //            new { NumeroConta = numeroConta }
    //        );

    //        return saldo ?? new SaldoDto();
    //    }
    //}

}
