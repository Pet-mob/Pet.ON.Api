using Pet.ON.Domain.Dtos.v1.Parametros;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1.Parametros
{
    public interface IParametrosServico
    {
        Task<BuscarParametroResDto> Buscar(int idEmpresa);
        Task<bool> Atualizar(AtualizarParametroReqDto parametros);
    }
}
