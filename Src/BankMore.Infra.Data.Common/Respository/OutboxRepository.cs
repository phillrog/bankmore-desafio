using BankMore.Domain.Common.Interfaces;
using BankMore.Domain.Common.Wrappers;
using BankMore.Infra.Data.Common.Notifications;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankMore.Infra.Data.Common.Respository
{
    public class OutboxRepository : IOutboxRepository
    {
        private const string ConnectionStringName = "DefaultConnection";
        private readonly IConfiguration _configuration;

        public OutboxRepository() { }
        public OutboxRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Add(INotification message)
        {
            #region [ SQL ]
            var sql = @"
            INSERT INTO OutboxMessages (Id, CreatedOn, Type, Payload, IsProcessed, CorrelationId)
            VALUES (@Id, @CreatedOn, @Type, @Payload, @IsProcessed, @CorrelationId);";
            #endregion

            var outboxMessage = new OutboxMessage(message);
            var connectionString = _configuration.GetConnectionString(ConnectionStringName);
            if (connectionString is null) return;

            using (IDbConnection conexao = new SqlConnection(connectionString))
            {
                conexao.Execute(sql, outboxMessage);
            }
        }

        public async Task<IEnumerable<OutboxMessageWrapper>> GetPendingMessagesAsync(int take)
        {
            var connectionString = _configuration.GetConnectionString(ConnectionStringName);
            if (connectionString is null) return Enumerable.Empty<OutboxMessageWrapper>();

            using IDbConnection db = new SqlConnection(connectionString);
            db.Open();

            #region [ SQL ]

            var sql = $@"
            SELECT TOP (@Take) Id, CreatedOn, Type, Payload, IsProcessed
            FROM OutboxMessages
            WHERE IsProcessed = 0
            ORDER BY CreatedOn ASC;";
            #endregion

            var wrappers = new List<OutboxMessageWrapper>();
            var pendingMessages = await db.QueryAsync<OutboxMessage>(sql, new { Take = take });

            var events = new List<INotification>();

            foreach (var message in pendingMessages)
            {
                var eventType = Type.GetType(message.Type);

                if (eventType is null) continue;

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                var eventInstance = JsonSerializer.Deserialize(message.Payload, eventType, options) as INotification;

                if (eventInstance is not null)
                {
                    events.Add(eventInstance);
                }

                wrappers.Add(new OutboxMessageWrapper
                {
                    OutboxId = message.Id,
                    Event = eventInstance
                });
            }

            return wrappers;
        }

        public async Task MarkAsProcessedAsync(IEnumerable<OutboxMessageWrapper> wrappers)
        {
            var connectionString = _configuration.GetConnectionString(ConnectionStringName);
            if (connectionString is null) return;

            var messageIds = wrappers.Select(m => m.OutboxId).ToList();

            if (!messageIds.Any()) return;

            using IDbConnection db = new SqlConnection(connectionString);
            db.Open();
            using var transaction = db.BeginTransaction();

            try
            {
                var sql = @"
                            UPDATE OutboxMessages
                            SET IsProcessed = 1
                            WHERE Id IN @Ids;";

                await db.ExecuteAsync(sql, new { Ids = messageIds }, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ExistTransfer(Guid correlationId)
        {
            var connectionString = _configuration.GetConnectionString(ConnectionStringName);
            if (connectionString is null) return false;

            #region [ SQL ]

            var sql = "SELECT COUNT(1) FROM outboxMessages WHERE CorrelationId = @CorrelationId;";
            #endregion

            using (IDbConnection conexao = new SqlConnection(connectionString))
            {
                int count = await conexao.QuerySingleAsync<int>(
                    sql,
                    new { CorrelationId = correlationId.ToString() }
                );

                return count > 0;
            }
        }
    }
}