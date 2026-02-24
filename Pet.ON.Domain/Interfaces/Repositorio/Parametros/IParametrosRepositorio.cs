using Pet.ON.Domain.Entidade.v1.Parametros;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Repositorio.Parametros
{
    public interface IParametrosRepositorio
    {
        Task<ParametroGeral> Buscar(int idEmpresa);
        Task<bool> Atualizar(ParametroGeral parametro);
    }
}
