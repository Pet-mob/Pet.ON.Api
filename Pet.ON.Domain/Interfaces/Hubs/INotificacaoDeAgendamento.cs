using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Hubs
{
    public interface INotificacaoDeAgendamento
    {
        Task NotificarNovoAgendamentoAsync(int idEmpresa, object dados);
    }
}
