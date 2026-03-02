using System;

namespace Pet.ON.Domain.Dtos.v1.Empresa
{
    public class BuscarHorariosFuncionamentosEmpresaReqDto
    {
        public int IdEmpresa { get; set; }
        public string NomeDiaSemana { get; set; }
        public bool FuncionaNesseDia { get; set; }
        public TimeSpan? HorarioAbertura { get; set; }
        public TimeSpan? HorarioFechamento { get; set; }
        public double IntervaloEntreServicos { get; set; }
    }
}
