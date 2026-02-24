using System;
using System.Collections.Generic;
using System.Text;

namespace Pet.ON.Domain.Dtos.v1
{
    public class BuscarAnimalReqDto
    {
        public int? IdAnimal { get; set; }
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public string TipoAnimal { get; set; }
    }
}
