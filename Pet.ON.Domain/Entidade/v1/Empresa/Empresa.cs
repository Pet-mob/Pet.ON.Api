namespace Pet.ON.Domain.Entidade.v1
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string DescricaoNomeFisica { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int IdCategoria { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public string UrlLogoEmpresa { get; set; } = string.Empty;
        public string UrlCapaEmpresa { get; set; } = string.Empty;
    }
}
