using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Animal;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Interfaces.Servico.v1;
using Pet.ON.Service.Servico;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : BaseController
    {
        private IAnimalServico _animalServico;
        public AnimalController(IAnimalServico animalServico)
        {
            _animalServico = animalServico;
        }

        #region GET
        /// <summary>
        /// Retornar uma lista de animais com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet]
        //[Authorize]
        [ProducesResponseType(typeof(List<BuscarAnimalResDto>), 200)]
        public async Task<IActionResult> Buscar([FromQuery] BuscarAnimalReqDto dto)
        {
            return ExecuteOperation(() => _animalServico.Buscar(dto));
        }
        #endregion

        [HttpPut]
        public async Task<IActionResult> AlterarAnimal([FromBody] AlterarAnimalReqDto dto)
        {
            return ExecuteOperation(() => _animalServico.Alterar(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Adcionar([FromBody] AdicionarAnimalReqDto dto)
        {
            return ExecuteOperation(() => _animalServico.Adicionar(dto));
        }
        
        [HttpPost("AdcionarAnimalNovo")]
        public async Task<IActionResult> AdcionarAnimalNovo([FromBody] AdicionarAnimalReqDto dto)
        {
            return ExecuteOperation(() => _animalServico.AdcionarAnimalNovo(dto));
        }

        [HttpDelete]
        public async Task<IActionResult> Excluir([FromQuery] int idUsuario, int idAnimal)
        {
            return ExecuteOperation(() => _animalServico.Excluir(idUsuario, idAnimal));
        }
    
        [HttpGet("BuscarFotosAnimaisPorUsuario")]
        public async Task<IActionResult> BuscarFotosAnimaisPorUsuario([FromQuery] BuscarFotoAnimalReqDto dto)
        {
            return ExecuteOperation(() => _animalServico.ListarFotosAnimaisPorUsuario(dto.IdUsuario));
        }

        [HttpPost("EnviarFotoAnimal")]
        public async Task<IActionResult> EnviarFotoDoUsuario(
            [FromForm] IFormFile arquivo,
            [FromForm] int idUsuario,
            [FromForm] int idAnimal)
        {
            if (arquivo == null)
                return BadRequest("Arquivo não foi recebido.");

            return ExecuteOperation(() => _animalServico.EnviarOuAtualizarFotoDoAnimal(arquivo, idUsuario, idAnimal));
        }
    }
}
