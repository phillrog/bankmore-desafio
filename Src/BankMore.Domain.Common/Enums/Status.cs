namespace BankMore.Domain.Common.Enums
{
    public enum StatusEnum
    {
        PENDENTE = 0,
        DEBITADO = 1,         // Débito realizado com sucesso na Conta Origem. Aguardando Crédito.
        TENTANDO_CREDITO = 2,
        CREDITADO = 3,         // Crédito realizado com sucesso na Conta Origem. Aguardando Conclusão.
        ESTORNO_PENDENTE = 4,
        ESTORNO_EFETUADO = 5,
        CONCLUIDA = 10,        // Realizado com sucesso (Saga completa). (Equivalente ao seu SUCESSO)
        // --- Falhas Iniciais ---
        ERRO_DEBITO = 11,     // O Débito falhou (Ex: Saldo Insuficiente, DebitoFalhouEvent).
        ERRO_CREDITO = 12,     // O Crédito falhou.

        // --- Falha e Compensação ---

        COMPENSADA_COM_FALHA = 20,
    }
}
