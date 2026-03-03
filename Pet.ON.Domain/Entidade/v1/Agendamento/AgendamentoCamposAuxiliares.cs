using System;

namespace Pet.ON.Domain.Entidade.v1
{
    public class AgendamentoCamposAuxiliares
    {
        public int IdAgendamento { get; set; }
        public string IdServicos { get; set; }
        public string DescricaoServicos { get; set; }
        public int IdAnimal { get; set; }
        public string NomeAnimal { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public bool PacoteMensal { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan HorarioInicial { get; set; }
        public TimeSpan HorarioFinal { get; set; }
        public int IdStatusAgendamento { get; set; }
        public string UrlFotoAnimal { get; set; }
    }
}
