using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Pet.ON.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : BaseController
    {

        private IEmpresaServico _empresaServico;
        public EmpresaController(IEmpresaServico empresaServico)
        {
            _empresaServico = empresaServico;
        }

        #region GET
        /// <summary>
        /// Retornar uma lista de empresa com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BuscarEmpresaResDto>), 200)]
        public async Task<IActionResult> Buscar([FromQuery] BuscarEmpresaReqDto dto)
        {
            return ExecuteOperation(() => _empresaServico.Buscar(dto));
        }

        [HttpGet("BuscarEmpresasVinculadoAoUsuario")]
        [ProducesResponseType(typeof(List<BuscarEmpresaResDto>), 200)]
        public async Task<IActionResult> BuscarEmpresasVinculadoAoUsuario([FromQuery] int idUsuario)
        {
            return ExecuteOperation(() => _empresaServico.BuscarEmpresasVinculadoAoUsuario(idUsuario));
        }
        
        /// <summary>
        /// Retornar uma lista de horarios de funcionamento por empresa com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet("HorariosFuncionamento")]
        [ProducesResponseType(typeof(List<BuscarHorariosFuncionamentosEmpresaResDto>), 200)]
        public async Task<IActionResult> BuscarHorariosFuncionamento([FromQuery] BuscarHorariosFuncionamentosEmpresaReqDto dto)
        {
            return ExecuteOperation(() => _empresaServico.BuscarHorariosFuncionamento(dto));
        }
        /// <summary>
        /// Retornar uma lista de horarios de funcionamento por empresa com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpPut("HorariosFuncionamento")]
        [ProducesResponseType(typeof(List<BuscarHorariosFuncionamentosEmpresaResDto>), 200)]
        public async Task<IActionResult> AtualizarHorariosFuncionamento([FromBody] List<BuscarHorariosFuncionamentosEmpresaReqDto> dto)
        {
            return ExecuteOperation(() => _empresaServico.AtualizarHorariosFuncionamento(dto));
        }

        #endregion

        #region Post
        /// <summary>
        /// Adicionar empresa.
        /// </summary>
        /// <returns>Retorna empresa</returns>
        [HttpPost]
        //[ProducesResponseType(typeof(), 200)]
        public async Task<IActionResult> AdicionarPetShop([FromBody] AdicionarEmpresaReqDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //400 bad request - solicitação inválida
            }

            try
            {
                var servico = _empresaServico.Cadastrar(dto);
                return Ok(servico.Result);

            }
            catch (ArgumentException e)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        #endregion

        #region Put
        /// <summary>
        /// atualizar empresa.
        /// </summary>
        /// <returns>Retorna empresa</returns>
        [HttpPut]
        //[Authorize]
        //[ProducesResponseType(typeof(), 200)]
        public async Task<IActionResult> AtualizarEmpresa([FromBody] AtualizarEmpresaReqDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //400 bad request - solicitação inválida
            }

            try
            {
                var servico = _empresaServico.Atualizar(dto);
                return Ok(servico.Result);

            }
            catch (ArgumentException e)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        #endregion

        [HttpGet("BuscarLogosEmpresas")]
        public async Task<IActionResult> BuscarLogos([FromQuery] int idEmpresa)
        {
            return ExecuteOperation(() => _empresaServico.ListarLogosEmpresa(idEmpresa));
        }
        
        [HttpGet("BuscarCapasEmpresas")]
        public async Task<IActionResult> BuscarCapas([FromQuery] int idEmpresa)
        {
            return ExecuteOperation(() => _empresaServico.ListarCapaEmpresa(idEmpresa));
        }

        [HttpPost("EnviarLogoEmpresa")]
        public async Task<IActionResult> EnviarLogos(
                        [FromForm] IFormFile arquivo,
            [FromForm] int idEmpresa)
        {
            return ExecuteOperation(() => _empresaServico.EnviarLogoEmpresa(arquivo, idEmpresa));
        }

        [HttpPost("EnviarCapaEmpresa")]
        public async Task<IActionResult> EnviarCapa(
                        [FromForm] IFormFile arquivo,
            [FromForm] int idEmpresa)
        {
            return ExecuteOperation(() => _empresaServico.EnviarCapaEmpresa(arquivo, idEmpresa));
        }
    }
}
