using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace Pet.ON.Api.Hubs
{
    public class AgendamentoHub : Hub
    {
        /// <summary>
        /// Chamado quando o cliente se conecta e fornece o ID da empresa
        /// </summary>
        /// <param name="empresaId">ID da empresa logada</param>
        public async Task EntrarNoGrupoDaEmpresa(int empresaId)
        {
            string grupo = ObterGrupoDaEmpresa(empresaId);
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
            Console.WriteLine($"Conexão {Context.ConnectionId} entrou no grupo {grupo}");
        }

        /// <summary>
        /// Remove o cliente do grupo da empresa ao sair ou desconectar
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        public async Task SairDoGrupoDaEmpresa(int empresaId)
        {
            string grupo = ObterGrupoDaEmpresa(empresaId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
            Console.WriteLine($"Conexão {Context.ConnectionId} saiu do grupo {grupo}");
        }

        /// <summary>
        /// Utilitário para nomear os grupos
        /// </summary>
        private string ObterGrupoDaEmpresa(int empresaId)
        {
            return $"empresa-{empresaId}";
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
        }
    }
}
