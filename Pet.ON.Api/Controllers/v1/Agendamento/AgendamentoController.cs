using Microsoft.AspNetCore.Mvc;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Agendamento;
using Pet.ON.Domain.Dtos.v1.Animal;
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
    public class AgendamentoController : BaseController
    {
        private IAgendamentoServico _agendamentoServico;
        public AgendamentoController(IAgendamentoServico agendamentoServico)
        {
            _agendamentoServico = agendamentoServico;
        }

        #region GET
        /// <summary>
        /// Retornar uma lista de agendamentos com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<BuscarAgendamentoResDto>), 200)]
        public IActionResult Buscar([FromQuery] BuscarAgendamentoReqDto dto)
        {
            return ExecuteOperation(() => _agendamentoServico.Buscar(dto));

        }

        /// <summary>
        /// Retornar uma lista de horarios disponiveis com base nos filtros.
        /// </summary>
        /// <returns>Retorna uma lista</returns>
        [HttpPost("HorariosDisponiveis")]
        [ProducesResponseType(typeof(BuscarHorariosDisponiveisResDto), 200)]
        public IActionResult BuscarHorariosDisponiveis([FromBody] BuscarHorariosDisponiveisReqDto dto)
        {
            return ExecuteOperation(() => _agendamentoServico.BuscarHorariosDisponiveis(dto));
        }
        #endregion

        /// <summary>
        /// Incluir agendamento
        /// </summary>
        /// <returns>a</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AdicionarAgendamentoResDto), 200)]
        public IActionResult IncluirAgendamento([FromBody] AdicionarAgendamentoReqDto dto)
        {
            return ExecuteOperation(() => _agendamentoServico.Adicionar(dto));
        }

        [HttpPost("Dashboard")]
        [ProducesResponseType(typeof(DashboardAgendamentoResDto), 200)]
        public IActionResult Dashboard([FromBody] DashboardAgendamentoReqDto dto)
        {
            return ExecuteOperation(() => _agendamentoServico.Dashboard(dto));
        }

        [HttpPost("Agenda")]        
        public IActionResult Agenda([FromBody] AgendaReqDto dto)
        {
            return ExecuteOperation(() => _agendamentoServico.Agenda(dto));
        }

        [HttpGet("QtdeAgendamentosDia")]
        public IActionResult QtdeAgendamentosDia([FromQuery] int idEmpresa, DateTime dataAgendamento, string horario)
        {            
            return ExecuteOperation(() => _agendamentoServico.BuscarQtdeAgendamentosDia(idEmpresa, dataAgendamento, horario));
        }

        [HttpGet("AgendamentosPendentes")]
        public IActionResult AgendamentosPendentes([FromQuery] int idEmpresa)
        {
            return ExecuteOperation(() => _agendamentoServico.BuscarAgendamentosPendentes(idEmpresa));
        }
        
        [HttpPut("AtualizarStatus")]
        public async Task<IActionResult> AtualizarStatusAgendamento([FromQuery] int idAgendamento, int status)
        {
            return ExecuteOperation(() => _agendamentoServico.AtualizarStatusAgendamento(idAgendamento, status));
        }
    }
}
