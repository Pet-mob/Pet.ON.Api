using System;
using System.Collections.Generic;
using System.Text;

namespace Pet.ON.Domain.Dtos.v1
{
    public class AtualizarEmpresaReqDto
    {
        public int IdEmpresa { get; set; }
        public string DescricaoNomeFisica { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }
}
