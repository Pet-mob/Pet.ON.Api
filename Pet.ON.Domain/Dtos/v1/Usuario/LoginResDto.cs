using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Dtos.v1.Usuario
{
    public class LoginResDto
    {
        public bool LoginAtivado { get; set; }
        public string Token { get; set; }
        public BuscarUsuarioResDto BuscarUsuarioResDto { get; set; }
    }
}
