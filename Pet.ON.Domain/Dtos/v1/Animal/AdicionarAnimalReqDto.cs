namespace Pet.ON.Domain.Dtos.v1.Animal
{
    public class AdicionarAnimalReqDto
    {
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public int IdPorte { get; set; }
        public string Observacoes { get; set; }
    }
}
