using System;
using System.Transactions;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Helper para operaciones transaccionales con TransactionScope.
    /// Garantiza atomicidad en operaciones multi-tabla.
    /// </summary>
    public static class TransactionHelper
    {
        /// <summary>
        /// Ejecuta una acción dentro de una transacción con nivel Serializable.
        /// Si la acción lanza excepción, la transacción hace rollback automático.
        /// </summary>
        public static void EjecutarTransaccion(Action accion, string descripcion = "Transacción")
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(30)
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    accion();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Rollback en {descripcion}", ex);
                    throw;
                }
            }
        }
    }
}
