using Microsoft.AspNetCore.Http;

namespace Pet.ON.Domain.Dtos.v1.Empresa
{
    public class EnviarLogoReqDto
    {
        public IFormFile Arquivo {  get; set; }
        public int IdEmpresa { get; set; }
    }
}
