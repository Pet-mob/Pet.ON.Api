using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Pet.ON.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : BaseController
    {
        private IServicosServico _servicosServico;
        public ServicosController(IServicosServico servicosServico)
        {
            _servicosServico = servicosServico;
        }

        #region GET
        /// <summary>
        /// Retornar uma lista de servicos de pet shop com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet("ListaServicosPetShop")]
        //[Authorize]
        [ProducesResponseType(typeof(List<BuscarServicosResDto>), 200)]
        public async Task<IActionResult> Buscar([FromQuery] BuscarServicosReqDto dto)
        {
            return ExecuteOperation(() => _servicosServico.Buscar(dto));
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Adicionar([FromBody] AdicionarServicoReqDto dto)
        {
            return ExecuteOperation(() => _servicosServico.Adicionar(dto));
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] AtualizarServicoReqDto dto)
        {
            return ExecuteOperation(() => _servicosServico.Atualizar(dto));
        }

        [HttpDelete]
        public async Task<IActionResult> Excluir([FromQuery] int idEmpresa, int idServico)
        {
            return ExecuteOperation(() => _servicosServico.Excluir(idEmpresa, idServico));
        }

        [HttpDelete("PossoExcluirServico")]        
        public async Task<IActionResult> PossoExcluirServico([FromQuery] int idEmpresa, int idServico)
        {
            return ExecuteOperation(() => _servicosServico.PossoExcluirServico(idEmpresa, idServico));
        }


    }
}
