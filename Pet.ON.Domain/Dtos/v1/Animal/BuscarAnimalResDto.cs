namespace Pet.ON.Domain.Dtos.v1
{
    public class BuscarAnimalResDto
    {
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public int Idade { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public string Observacoes { get; set; }
        public int IdPorte { get; set; }
        public string UrlFotoAnimal { get; set; }
    }
}
