using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pet.ON.Service.Servico
{
    public class ServicosServico : IServicosServico
    {
        private readonly IMapper _mapper;
        private readonly IServicosRepositorio _servicosRepositorio;

        public ServicosServico(IServicosRepositorio servicosRepositorio, IMapper mapper)
        {
            _mapper = mapper;
            _servicosRepositorio = servicosRepositorio;
        }

        public Task Adicionar(AdicionarServicoReqDto dto)
        {
            return _servicosRepositorio.Adicionar(dto);
        }

        public Task Atualizar(AtualizarServicoReqDto dto)
        {
            return _servicosRepositorio.Atualizar(dto);
        }

        public async Task<IEnumerable<BuscarServicosResDto>> Buscar(BuscarServicosReqDto dto)
        {
            // Garante que a busca sempre considere idPorte = 0 também
            var servicos = await _servicosRepositorio.GetByParameters(dto);
            return _mapper.Map<IEnumerable<BuscarServicosResDto>>(servicos);
        }

        public async Task<bool> Excluir(int idEmpresa, int idServico)
        {
            var servicos = await _servicosRepositorio.Excluir(idEmpresa, idServico);
            return _mapper.Map<bool>(servicos);

        }

        public async Task<bool> PossoExcluirServico(int idEmpresa, int idServico)
        {
            var servicos = await _servicosRepositorio.PossoExcluirServico(idEmpresa, idServico);
            return _mapper.Map<bool>(servicos);
        }
    }
}
