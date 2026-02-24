using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Repositorio
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario> BuscarPorTelefone(string telefone);
        Task<Usuario> BuscarPorCnpj(string cnpj); // 🔥 Adicionamos essa função
        Task<IEnumerable<Usuario>> GetByParameters(BuscarUsuarioReqDto dto);
        Task<int> InserirUsuarioAsync(Usuario usuario);
        Task<bool> AlterarSenhaDoUsuario(string senhaNova, int idUsuario);
        Task<bool> AlterarUsuario(string nome, string telefone, int idUsuario, string email);
        Task<bool> Excluir(int idUsuario);
    }
}
