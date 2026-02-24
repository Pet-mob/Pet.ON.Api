using System;

namespace Pet.ON.Domain.Dtos.v1.Agendamento
{
    public class AgendaReqDto
    {
        public DateTime? DataFiltroInicio { get; set; }
        public DateTime? DataFiltroFim { get; set; }
        public int? IdEmpresa { get; set; }
    }
}
