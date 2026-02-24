using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Repositorio
{
    public interface IServicosRepositorio
    {
        Task Adicionar(AdicionarServicoReqDto dto);
        Task Atualizar(AtualizarServicoReqDto dto);
        Task<IEnumerable<Servicos>> GetByParameters(BuscarServicosReqDto dto);
        Task<bool> Excluir(int idEmpresa, int idServico);
        Task<bool> PossoExcluirServico(int idEmpresa, int idServico);
    }
}
