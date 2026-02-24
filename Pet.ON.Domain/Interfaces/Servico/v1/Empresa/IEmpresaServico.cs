using Microsoft.AspNetCore.Http;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Empresa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IEmpresaServico
    {
        Task<IEnumerable<BuscarEmpresaResDto>> Buscar(BuscarEmpresaReqDto dto);
        Task<AdicionarEmpresaResDto> Cadastrar(AdicionarEmpresaReqDto dto);
        Task<AtualizarEmpresaResDto> Atualizar(AtualizarEmpresaReqDto dto);
        Task<IEnumerable<BuscarHorariosFuncionamentosEmpresaResDto>> BuscarHorariosFuncionamento(BuscarHorariosFuncionamentosEmpresaReqDto dto);
        Task<List<BuscarLogosResDto>> ListarLogosEmpresa(int idEmpresaFiltro);
        Task<string> EnviarLogoEmpresa(IFormFile arquivo, int idEmpresa);
        Task AtualizarHorariosFuncionamento(List<BuscarHorariosFuncionamentosEmpresaReqDto> dto);
        Task<List<BuscarLogosResDto>> ListarCapaEmpresa(int idEmpresa);
        Task<string> EnviarCapaEmpresa(IFormFile arquivo, int idEmpresa);
        Task<IEnumerable<BuscarEmpresaResDto>> BuscarEmpresasVinculadoAoUsuario(int idUsuario);
    }
}
