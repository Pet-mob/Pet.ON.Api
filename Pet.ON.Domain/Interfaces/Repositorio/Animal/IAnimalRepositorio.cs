using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Repositorio
{
    public interface IAnimalRepositorio
    {
        Task<IEnumerable<Animal>> GetByParameters(BuscarAnimalReqDto dto);
        Task<bool> AlterarAnimal(string nome, int idade, string raca, string observacoes, int idAnimal, int idUsuario, int idPorte);
        Task<bool> Adicionar(Animal animal);
        Task<bool> Excluir(int idUsuario, int idAnimal);
        Task<int> AdicionarAnimalNovo(Animal animal);
    }
}
