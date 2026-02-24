namespace Pet.ON.Domain.Dtos.v1.Usuario
{
    public class AlterarUsuarioReqDto
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
}
