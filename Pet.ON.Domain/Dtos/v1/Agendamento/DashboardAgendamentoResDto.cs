namespace Pet.ON.Domain.Dtos.v1.Agendamento
{
    public class DashboardAgendamentoResDto
    {
        public int PetsAgendadosHoje { get; set; }
        public int ServicosConcluidosHoje { get; set; }
        public int AgendamentosAmanha { get; set; }
        public string ProximoHorario { get; set; }
        public int[] GraficoSemanal { get; set; }
    }
}
