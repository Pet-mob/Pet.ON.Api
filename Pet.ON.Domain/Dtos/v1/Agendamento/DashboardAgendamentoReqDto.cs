using System;

namespace Pet.ON.Domain.Dtos.v1.Agendamento
{
    public class DashboardAgendamentoReqDto
    {
        public DateTime DataFiltro { get; set; }
        public int IdEmpresa { get; set; }
    }
}
