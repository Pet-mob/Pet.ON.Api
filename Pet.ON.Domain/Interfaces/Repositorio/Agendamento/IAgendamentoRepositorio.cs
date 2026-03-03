using Pet.ON.Domain.Entidade.v1;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace Pet.ON.Domain.Interfaces.Repositorio
{
    public interface IAgendamentoRepositorio
    {
        Task<IEnumerable<Agendamento>> BuscarAgendamentosPorDia(int idEmpresa, DateTime dataAgendamento);
        Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgendamentos(int idUsuario);
        Task<int> Adicionar(Agendamento agendamento, bool primeiro);
        Task AtualizarIdAgendamentoPai(int id);
        Task<DashboardAgendamentos> DashboardAgendamento(DateTime dataFiltro, int idEmpresa);
        Task<int[]> GraficoSemanal(DateTime inicioSemana, DateTime fimSemana, int idEmpresa);
        Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgenda(DateTime? dataFiltroInicio, DateTime? dataFiltroFim, int? idEmpresa);
        Task<int> BuscarQtdeAgendamentosDia(int idEmpresa, DateTime dataAgendamento, string horario);
        Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgendamentosPendentes(int idEmpresa);
        Task<bool> AtualizarStatusAgendamento(int idAgendamento, int status);
    }
}
