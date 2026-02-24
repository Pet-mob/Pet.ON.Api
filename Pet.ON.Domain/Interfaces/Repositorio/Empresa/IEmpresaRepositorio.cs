using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Pet.ON.Domain.Entidade.v1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Repositorio
{
    public interface IEmpresaRepositorio
    {
        Task<IEnumerable<Empresa>> GetByParameters(BuscarEmpresaReqDto empresa);
        Task<Empresa> UpdateAsync(Empresa empresa);
        Task<Empresa> InsertAsync(Empresa empresa);
        Task<IEnumerable<HorariosFuncionamento>> BuscarHorariosFuncionamento(BuscarHorariosFuncionamentosEmpresaReqDto dto);
        Task AtualizarHorariosFuncionamentoAsync(List<HorariosFuncionamento> horarios);
        Task<IEnumerable<Empresa>> BuscarEmpresasVinculadoAoUsuario(int idUsuario);
    }
}
