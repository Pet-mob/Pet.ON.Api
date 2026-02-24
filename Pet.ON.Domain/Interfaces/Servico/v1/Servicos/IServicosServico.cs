using Pet.ON.Domain.Dtos.v1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IServicosServico
    {
        Task Adicionar(AdicionarServicoReqDto dto);
        Task Atualizar(AtualizarServicoReqDto dto);
        Task<IEnumerable<BuscarServicosResDto>> Buscar(BuscarServicosReqDto dto);
        Task<bool> Excluir(int idEmpresa, int idServico);
        Task<bool> PossoExcluirServico(int idEmpresa, int idServico);
    }
}
