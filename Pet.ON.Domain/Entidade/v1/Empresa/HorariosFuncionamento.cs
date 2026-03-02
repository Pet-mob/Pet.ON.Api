using System;

namespace Pet.ON.Domain.Entidade.v1
{
    public class HorariosFuncionamento
    {
        public int IdEmpresa { get; set; }
        public string NomeDiaSemana { get; set; }
        public bool FuncionaNesseDia { get; set; }
        public TimeSpan? HorarioAbertura { get; set; }
        public TimeSpan? HorarioFechamento { get; set; }
        public double IntervaloEntreServicos { get; set; }
    }
}
