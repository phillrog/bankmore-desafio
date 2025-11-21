namespace BankMore.Application.Common.Topicos.Saga
{
    public static class SagaTopico
    {
        /// <summary>
        /// Topico: saga.transferencia.iniciar
        /// </summary>
        public static string IniciarTranferencia = "saga.transferencia.iniciar"; // TransferenciaIniciadaOrquestradorHandler


        /// <summary>
        /// Topico: "saga.contacorrente.debitar"
        /// </summary>
        public static string DebitarConta = "saga.contacorrente.debitar"; // DebitarContaConsumer

        /// <summary>
        /// Topico: "saga.transferencia.validar.debito"
        /// </summary>
        public static string ValidarDebitoConta = "saga.transferencia.validar.debito"; // ValidarDebitoConsumer

        /// <summary>
        /// Topico: "saga.contacorrente.creditar"
        /// </summary>
        public static string CreditarConta = "saga.contacorrente.creditar"; // TentarCreditoContaConsumer

        /// <summary>
        /// Topico: "saga.contacorrente.estornar"
        /// </summary>
        public static string EstornarConta = "saga.contacorrente.estornar"; // EstornaDebitoContaConsumer

        /// <summary>
        /// Topico: "saga.contacorrente.finalizar"
        /// </summary>
        public static string FinalizadaTransferencia = "saga.contacorrente.finalizar"; // TransferenciaFinalizadaConsumer

        /// <summary>
        /// Topico: "grupo.saga.transferencia"
        /// </summary>
        public static string TranferenciaGrupo = "grupo.saga.transferencia"; // API Transferencia
        /// <summary>
        /// Topico: "grupo.saga.contacorrente"
        /// </summary>
        public static string ContaCorrenteGrupo = "grupo.saga.contacorrente"; // API ContaCorrente
    }
}
