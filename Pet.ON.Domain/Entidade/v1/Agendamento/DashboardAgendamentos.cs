namespace Pet.ON.Domain.Entidade.v1
{
    public class DashboardAgendamentos
    {
        public int PetsAgendadosHoje { get; set; }
        public int ServicosConcluidosHoje { get; set; }
        public int AgendamentosAmanha { get; set; }
        public string ProximoHorario { get; set; }
    }
}
