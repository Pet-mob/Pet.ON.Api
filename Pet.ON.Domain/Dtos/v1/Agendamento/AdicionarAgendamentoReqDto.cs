using System;
using System.Collections.Generic;

namespace Pet.ON.Domain.Dtos.v1
{
    public class AdicionarAgendamentoReqDto
    {
        public int IdAgendamento { get; set; }
        public int[] IdServicos { get; set; }
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public bool PacoteMensal {  get; set; }
        public List<DateTime> ListaDatasAgendamento { get; set; }
        public TimeSpan Horario { get; set; }
        public TimeSpan HorarioFinal { get; set; }
        public int IdStatusAgendamento { get; set; }        
    }
}
