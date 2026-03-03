using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Service.Servico
{
    /// <summary>
    /// Serviço para gerenciar operações de serviços oferecidos pelas empresas.
    /// Responsabilidades: CRUD de serviços e validação de exclusão.
    /// </summary>
    public class ServicosServico : IServicosServico
    {
        private readonly IMapper _mapper;
        private readonly IServicosRepositorio _servicosRepositorio;

        public ServicosServico(IServicosRepositorio servicosRepositorio, IMapper mapper)
        {
            _servicosRepositorio = servicosRepositorio ?? throw new ArgumentNullException(nameof(servicosRepositorio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Adiciona um novo serviço.
        /// </summary>
        public async Task Adicionar(AdicionarServicoReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de serviço não podem ser nulos.");

            try
            {
                await _servicosRepositorio.Adicionar(dto);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao adicionar serviço.", ex);
            }
        }

        /// <summary>
        /// Atualiza os dados de um serviço existente.
        /// </summary>
        public async Task Atualizar(AtualizarServicoReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de serviço não podem ser nulos.");

            try
            {
                await _servicosRepositorio.Atualizar(dto);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar serviço.", ex);
            }
        }

        /// <summary>
        /// Busca serviços com filtros aplicados.
        /// </summary>
        public async Task<IEnumerable<BuscarServicosResDto>> Buscar(BuscarServicosReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Filtros de busca não podem ser nulos.");

            try
            {
                var servicos = await _servicosRepositorio.GetByParameters(dto);
                return _mapper.Map<IEnumerable<BuscarServicosResDto>>(servicos);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar serviços.", ex);
            }
        }

        /// <summary>
        /// Exclui um serviço especificado.
        /// </summary>
        /// <param name="idEmpresa">ID da empresa proprietária do serviço</param>
        /// <param name="idServico">ID do serviço a excluir</param>
        /// <returns>True se exclusão foi bem-sucedida, False caso contrário</returns>
        public async Task<bool> Excluir(int idEmpresa, int idServico)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");
            ValidadorArquivo.ValidarId(idServico, "serviço");

            try
            {
                return await _servicosRepositorio.Excluir(idEmpresa, idServico);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao excluir serviço.", ex);
            }
        }

        /// <summary>
        /// Verifica se um serviço pode ser excluído (sem agendamentos associados).
        /// </summary>
        /// <param name="idEmpresa">ID da empresa proprietária do serviço</param>
        /// <param name="idServico">ID do serviço a verificar</param>
        /// <returns>True se pode ser excluído, False caso contrário</returns>
        public async Task<bool> PossoExcluirServico(int idEmpresa, int idServico)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");
            ValidadorArquivo.ValidarId(idServico, "serviço");

            try
            {
                return await _servicosRepositorio.PossoExcluirServico(idEmpresa, idServico);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao validar exclusão de serviço.", ex);
            }
        }
    }
}
