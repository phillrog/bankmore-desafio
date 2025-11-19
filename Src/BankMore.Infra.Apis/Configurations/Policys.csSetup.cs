using BankMore.Infra.CrossCutting.Identity.Authorization;
using BankMore.Infra.CrossCutting.Identity.Filters;
using Microsoft.AspNetCore.Authorization;

namespace BankMore.Infra.Apis.Configurations
{
    /// <summary>
    /// Configuração de polÃ­ticas de acesso e permissão
    /// </summary>
    public static class PolicySetup
    {
        #region [ CONSTANTES ]

        // Define os nomes das polÃ­ticas como constantes para evitar erros de digitação.
        public const string MasterAccess = "MasterAccess";
        public const string CanWriteData = "CanWriteData";
        public const string CanRemoveData = "CanRemoveData";
        public const string CanReadData = "CanReadData";
        
        // PolÃ­ticas de filtragem de recurso
        public const string OwnerOrMaster_CPF_Policy = "OwnerOrMaster_CPF";
        public const string OwnerOrMaster_Conta_Policy = "OwnerOrMaster_Conta";

        public const string CanWriteDataOrMasterPolicy = "CanWriteDataOrMaster";
        public const string CanReadDataOrMasterPolicy = "CanReadDataOrMaster";
        public const string CanRemoveDataOrMasterPolicy = "CanRemoveDataOrMaster";
        #endregion

        /// <summary>
        /// Adiciona e configura todas as polÃ­ticas de autorização do sistema.
        /// </summary>
        /// <param name="options">As opçÃµes de autorização a serem configuradas.</param>
        public static void AddCustomPolicies(AuthorizationOptions options)
        {
            var readRequirement = new ClaimRequirement("Admin_Read", "Read");
            var writeRequirement = new ClaimRequirement("Admin_Write", "Write");
            var removeRequirement = new ClaimRequirement("Admin_Remove", "Remove");

            #region [ POLÃTICAS BASE (Admin + Claim especÃ­fico) ]

            options.AddPolicy(MasterAccess, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Master");
            });

            options.AddPolicy(CanWriteData, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
                policy.AddRequirements(writeRequirement);
            });

            options.AddPolicy(CanRemoveData, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
                policy.AddRequirements(removeRequirement);
            });

            options.AddPolicy(CanReadData, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
                policy.AddRequirements(readRequirement);
            });

            #endregion

            #region [ DEFINIÇÕO CENTRALIZADA DA POLÃTICA DE FILTRAGEM DE RECURSOS ]
            
            // 1. POLÃTICA PARA ENDPOINTS COM {cpf} NA ROTA
            options.AddPolicy(OwnerOrMaster_CPF_Policy, policy => // Usando a constante
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Master", "Admin");
                // Passa "cpf" como a chave a ser procurada na rota
                policy.AddRequirements(new MustBeOwnerOrMasterRequirement("cpf", "numero_conta"));
            });

            // 2. POLÃTICA PARA ENDPOINTS COM {numeroConta} NA ROTA
            options.AddPolicy(OwnerOrMaster_Conta_Policy, policy => // Usando a constante
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Master", "Admin");
                // Passa "numeroConta" como a chave a ser procurada na rota
                policy.AddRequirements(new MustBeOwnerOrMasterRequirement("numeroConta", "numero_conta"));
            });

            #endregion


            #region [POLÃTICAS COMBINADAS (MASTER OU ADMIN COM CLAIM)]

            options.AddPolicy(CanReadDataOrMasterPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Master") ||
                    (context.User.IsInRole("Admin") && context.User.HasClaim(readRequirement.ClaimName, readRequirement.ClaimValue))
                );
            });

            options.AddPolicy(CanWriteDataOrMasterPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Master") ||
                    (context.User.IsInRole("Admin") && context.User.HasClaim(writeRequirement.ClaimName, writeRequirement.ClaimValue))
                );
            });

            options.AddPolicy(CanRemoveDataOrMasterPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Master") ||
                    (context.User.IsInRole("Admin") && context.User.HasClaim(removeRequirement.ClaimName, removeRequirement.ClaimValue))
                );
            });

            #endregion 
        }
    }
}
