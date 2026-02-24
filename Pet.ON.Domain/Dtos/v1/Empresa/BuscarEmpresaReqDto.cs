namespace Pet.ON.Domain.Dtos.v1
{
    public class BuscarEmpresaReqDto
    {
        public string Cnpj { get; set; }
        public int? IdEmpresa { get; set; }
        public string DescricaoNomeFantasia { get; set; }
        public int? IdCategoria { get; set; }
    }
}
