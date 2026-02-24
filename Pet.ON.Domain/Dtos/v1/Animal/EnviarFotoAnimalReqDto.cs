using Microsoft.AspNetCore.Http;

namespace Pet.ON.Domain.Dtos.v1.Animal
{
    public class EnviarFotoAnimalReqDto
    {
        public IFormFile Arquivo { get; set; }
        public int IdAnimal { get; set; }
        public int IdUsuario { get; set; }
    }
}
