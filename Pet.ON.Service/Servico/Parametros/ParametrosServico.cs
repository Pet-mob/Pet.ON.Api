using AutoMapper;
using Pet.ON.Domain.Dtos.v1.Parametros;
using Pet.ON.Domain.Entidade.v1.Parametros;
using Pet.ON.Domain.Interfaces.Repositorio.Parametros;
using Pet.ON.Domain.Interfaces.Servico.v1.Parametros;
using System.Threading.Tasks;

namespace Pet.ON.Service.Servico.Parametros
{
    public class ParametrosServico : IParametrosServico
    {
        private readonly IParametrosRepositorio _parametrosRepositorio;
        private readonly IMapper _mapper;

        public ParametrosServico(IParametrosRepositorio parametrosRepositorio, IMapper mapper)
        {
            _parametrosRepositorio = parametrosRepositorio;
            _mapper = mapper;
        }

        public async Task<BuscarParametroResDto> Buscar(int idEmpresa)
        {
            var parametro = await _parametrosRepositorio.Buscar(idEmpresa);
            return _mapper.Map<BuscarParametroResDto>(parametro);
        }

        public async Task<bool> Atualizar(AtualizarParametroReqDto parametros)
        {

            var atualizar = await _parametrosRepositorio.Atualizar(new ParametroGeral
            {
                IdEmpresa = parametros.IdEmpresa,
                QtdeAtendimentoSimultaneoHorario = parametros.QtdeAtendimentoSimultaneoHorario,
                IdModeloTrabalho = parametros.IdModeloTrabalho
            });

            return _mapper.Map<bool>(atualizar);
        }
    }
}
