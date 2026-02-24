using Microsoft.AspNetCore.SignalR;
using Pet.ON.Api.Hubs;
using Pet.ON.Domain.Interfaces.Hubs;
using System.Threading.Tasks;

namespace Pet.ON.Api.Notificacoes
{
    public class NotificacaoDeAgendamentoSignalR : INotificacaoDeAgendamento
    {
        private readonly IHubContext<AgendamentoHub> _hubContext;

        public NotificacaoDeAgendamentoSignalR(IHubContext<AgendamentoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarNovoAgendamentoAsync(int idEmpresa, object dados)
        {
            await _hubContext.Clients.Group($"empresa-{idEmpresa}").SendAsync("NovoAgendamento", dados);
        }
    }
}
