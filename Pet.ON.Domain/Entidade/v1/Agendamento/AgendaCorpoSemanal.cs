using System;

namespace Pet.ON.Domain.Entidade.v1
{
    public class AgendaCorpoSemanal
    {
        public string HorarioInicio { get; set; } = null!;
        public string Pet { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public DateTime Data { get; set; }
    }
}
