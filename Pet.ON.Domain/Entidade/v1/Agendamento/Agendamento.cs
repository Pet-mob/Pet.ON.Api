using System;

namespace Pet.ON.Domain.Entidade.v1
{
    public class Agendamento
    {
        public int IdAgendamento { get; set; }
        public int IdServico { get; set; }
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public bool PacoteMensal { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HorarioInicial { get; set; }
        public TimeSpan HorarioFinal { get; set; }
        public string Status { get; set; }
        public int? IdAgendamentoPai { get; set; }

    }
}
