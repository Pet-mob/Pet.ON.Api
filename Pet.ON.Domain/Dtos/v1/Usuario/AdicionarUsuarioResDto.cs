using System;

namespace Pet.ON.Domain.Dtos.v1
{
    public class AdicionarUsuarioResDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string Geolocalizacao { get; set; }
        public DateTime DataPrimeiroAcesso { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdAnimal { get; set; }
    }
}
