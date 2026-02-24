using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Interfaces.Servico.v1;
using Pet.ON.Service.Servico;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Pet.ON.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : BaseController
    {

        private IUsuarioServico _usuarioServico;
        public UsuarioController(IUsuarioServico usuarioServico)
        {
            _usuarioServico = usuarioServico;
        }


        /// <summary>
        /// Retornar uma lista de usuarios com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BuscarUsuarioResDto>), 200)]
        public IActionResult Buscar([FromQuery] BuscarUsuarioReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.Buscar(dto));
        }

        /// <summary>
        /// Adicionar usuario.
        /// </summary>
        /// <returns>Retorna usuario</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AdicionarUsuarioResDto), 200)]
        public async Task<IActionResult> Adicionar([FromBody] AdicionarUsuarioReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.Cadastrar(dto));
        }


        #region Post
        /// <summary>
        /// Adicionar usuario e criando o cadastro do pet shop.
        /// </summary>
        /// <returns>Retorna usuario</returns>
        [HttpPost("AdicionarUsuarioPetShop")]
        //[Authorize]
        [ProducesResponseType(typeof(AdicionarUsuarioResDto), 200)]
        public async Task<IActionResult> AdicionarUsuarioPetShop([FromBody] AdicionarUsuarioReqDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //400 bad request - solicitação inválida
            }

            try
            {
                var servico = _usuarioServico.CadastrarUsuarioPetShop(dto);
                return Ok(servico.Result);

            }
            catch (ArgumentException e)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        #endregion

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.Login(dto));
        }

        [HttpPut("AlterarSenhaDoUsuario")]
        public async Task<IActionResult> AlterarSenhaDoUsuario([FromBody] AlterarSenhaReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.AlterarSenhaDoUsuario(dto));
        }

        [HttpPut]
        public async Task<IActionResult> AlterarUsuario([FromBody] AlterarUsuarioReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.AlterarUsuario(dto));
        }

        [HttpGet("BuscarFotosUsuario")]
        public async Task<IActionResult> BuscarFotosDoUsuario([FromQuery] BuscarFotoUsuarioReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.ListarFotosDoUsuarios(dto.IdUsuario));
        }

        [HttpPost("EnviarFotoDoUsuario")]
        public async Task<IActionResult> EnviarFotoDoUsuario(
            [FromForm] IFormFile arquivo,
            [FromForm] int idUsuario)
        {
            if (arquivo == null)
                return BadRequest("Arquivo não foi recebido.");

            return ExecuteOperation(() => _usuarioServico.EnviarOuAtualizarFotoDoUsuario(arquivo, idUsuario));
        }

        [HttpPost("AdicionarUsuarioNovo")]
        public async Task<IActionResult> AdicionarUsuarioNovo([FromBody] AdicionarUsuarioReqDto dto)
        {
            return ExecuteOperation(() => _usuarioServico.AdicionarUsuarioNovo(dto));
        }

        [HttpDelete]
        public async Task<IActionResult> Excluir([FromQuery] int idUsuario)
        {
            return ExecuteOperation(() => _usuarioServico.Excluir(idUsuario));
        }

        [HttpGet("ValidarTelefoneCadastrado")]
        public async Task<IActionResult> ValidarTelefoneCadastrado([FromQuery] string telefone)
        {
            return ExecuteOperation(() => _usuarioServico.ValidarTelefoneCadastrado(telefone));
        }

    }
}