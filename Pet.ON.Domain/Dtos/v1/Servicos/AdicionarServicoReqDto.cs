namespace Pet.ON.Domain.Dtos.v1
{
    public class AdicionarServicoReqDto
    {
        public int IdServico { get; set; }
        public int IdEmpresa { get; set; }
        public string Descricao { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Duracao { get; set; }
        public string Observacao { get; set; }
        public bool PossuiMensal { get; set; } = false;
        public decimal? PrecoMensal { get; set; }
        public int IdPorte { get; set; }
    }
}
