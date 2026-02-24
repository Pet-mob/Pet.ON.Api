using System;
using System.Collections.Generic;

namespace Pet.ON.Domain.Dtos.v1.Agendamento
{
    public class BuscarHorariosDisponiveisReqDto
    {
        public int IdEmpresa { get; set; }
        public List<DateTime> ListaDataAgendamento { get; set; }
        public int DuracaoEmMinutos { get; set; }
        public TimeSpan HorarioAtual { get; set; }
    }
}
