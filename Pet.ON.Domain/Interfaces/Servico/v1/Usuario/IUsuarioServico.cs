using Microsoft.AspNetCore.Http;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IUsuarioServico
    {
        Task<LoginResDto> Login(LoginReqDto dto);
        Task<IEnumerable<BuscarUsuarioResDto>> Buscar(BuscarUsuarioReqDto dto);
        Task Cadastrar(AdicionarUsuarioReqDto dto);
        Task<AdicionarUsuarioResDto> CadastrarUsuarioPetShop(AdicionarUsuarioReqDto dto);
        Task<bool> AlterarSenhaDoUsuario(AlterarSenhaReqDto dto);
        Task<bool> AlterarUsuario(AlterarUsuarioReqDto dto);
        Task<string> EnviarOuAtualizarFotoDoUsuario(IFormFile arquivo, int idUsuario);
        Task<List<BuscarFotoUsuarioResDto>> ListarFotosDoUsuarios(int idUsuarioFiltro);
        Task<int> AdicionarUsuarioNovo(AdicionarUsuarioReqDto dto);
        Task<bool> Excluir(int idUsuario);
        Task<bool> ValidarTelefoneCadastrado(string telefone);
    }
}
