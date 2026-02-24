using Microsoft.AspNetCore.Http;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Animal;
using Pet.ON.Domain.Dtos.v1.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IAnimalServico
    {
        Task<bool> Adicionar(AdicionarAnimalReqDto dto);
        Task<int> AdcionarAnimalNovo(AdicionarAnimalReqDto dto);
        Task<bool> Alterar(AlterarAnimalReqDto dto);
        Task<IEnumerable<BuscarAnimalResDto>> Buscar(BuscarAnimalReqDto dto);
        Task<string> EnviarOuAtualizarFotoDoAnimal(IFormFile arquivo, int idUsuario, int idAnimal);
        Task<bool> Excluir(int idUsuario, int idAnimal);
        Task<List<BuscarFotoAnimalResDto>> ListarFotosAnimaisPorUsuario(int idUsuarioFiltro);
    }
}
