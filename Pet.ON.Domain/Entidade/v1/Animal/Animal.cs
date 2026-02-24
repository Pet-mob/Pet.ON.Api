namespace Pet.ON.Domain.Entidade.v1
{
    public class Animal
    {
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public int Idade { get; set; }
        public string Observacoes { get; set; }
        public int IdPorte { get; set; } // Supondo que IdPorte seja uma propriedade de Animal
        public string UrlFotoAnimal { get; set; }

    }
}