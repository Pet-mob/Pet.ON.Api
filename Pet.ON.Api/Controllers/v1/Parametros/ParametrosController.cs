
using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Parametros;
using Pet.ON.Domain.Interfaces.Servico.v1.Parametros;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Api.Controllers.v1.Parametros
{
    [Route("api/[controller]")]
    public class ParametrosController : BaseController
    {
        private readonly IParametrosServico _parametrosServico;

        public ParametrosController(IParametrosServico parametrosServico)
        {
            _parametrosServico = parametrosServico;
        }

        [HttpGet]
        public async Task<IActionResult> Buscar([FromQuery] int idEmpresa)
        {
            return ExecuteOperation(() => _parametrosServico.Buscar(idEmpresa));
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] AtualizarParametroReqDto parametros)
        {
            return ExecuteOperation(() => _parametrosServico.Atualizar(parametros));
        }
    }
}
