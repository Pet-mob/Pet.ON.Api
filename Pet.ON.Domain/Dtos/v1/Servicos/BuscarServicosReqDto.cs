using System;
using System.Collections.Generic;
using System.Text;

namespace Pet.ON.Domain.Dtos.v1
{
    public class BuscarServicosReqDto
    {        
        public int IdServico { get; set; }
        public int IdEmpresa { get; set; }
        public int IdPorte { get; set; }
    }
}
