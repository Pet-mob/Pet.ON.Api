using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Agendamento;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IAgendamentoServico
    {
        Task<List<BuscarAgendamentoResDto>> Buscar(BuscarAgendamentoReqDto dto);
        Task<AdicionarAgendamentoResDto> Adicionar(AdicionarAgendamentoReqDto dto);
        Task<BuscarHorariosDisponiveisResDto> BuscarHorariosDisponiveis(BuscarHorariosDisponiveisReqDto dto);
        Task<DashboardAgendamentoResDto> Dashboard(DashboardAgendamentoReqDto dto);
        Task<List<AgendaDiaResDto>> Agenda(AgendaReqDto dto);
        Task<int> BuscarQtdeAgendamentosDia(int idEmpresa, DateTime dataAgendamento, string horario);
        Task<List<BuscarAgendamentoResDto>> BuscarAgendamentosPendentes(int idEmpresa);
        Task<bool> AtualizarStatusAgendamento(int idAgendamento, int status);
    }
}
