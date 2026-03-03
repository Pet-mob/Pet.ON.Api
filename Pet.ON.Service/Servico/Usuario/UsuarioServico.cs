using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Utilitarios.Validacoes;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Pet.ON.Service.Servico
{
    /// <summary>
    /// Serviço para gerenciar operações de usuário.
    /// Responsabilidades: CRUD de usuário, autenticação e gerencimento de foto de perfil.
    /// </summary>
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        private readonly IEmpresaServico _empresaServico;
        private readonly IStorageService _storageService;
        private readonly GerenciadorMidiaSobreEscrita _gerenciadorMidia;

        public UsuarioServico(
            IMapper mapper,
            IEmpresaServico empresaServico,
            IUsuarioRepositorio usuarioRepositorio,
            IStorageService storageService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _empresaServico = empresaServico ?? throw new ArgumentNullException(nameof(empresaServico));
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _gerenciadorMidia = new GerenciadorMidiaSobreEscrita(_storageService);
        }

        #region Cadastro

        /// <summary>
        /// Cadastra um novo usuário com validações de negócio.
        /// </summary>
        public async Task Cadastrar(AdicionarUsuarioReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de cadastro não podem ser nulos.");

            try
            {
                var usuario = _mapper.Map<Usuario>(dto);

                var validador = new Validador<Usuario>();
                if (!validador.EstaValido(usuario))
                {
                    var notificacoes = validador.ObterNotificacoes();
                    throw new InvalidOperationException($"Usuário inválido: {string.Join("; ", notificacoes)}");
                }

                await _usuarioRepositorio.InserirUsuarioAsync(usuario);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao cadastrar usuário.", ex);
            }
        }

        /// <summary>
        /// Cadastra um novo usuário PetShop criando também uma empresa vinculada.
        /// </summary>
        public async Task<AdicionarUsuarioResDto> CadastrarUsuarioPetShop(AdicionarUsuarioReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de cadastro não podem ser nulos.");

            try
            {
                var usuario = _mapper.Map<Usuario>(dto);
                
                var empresaDto = new AdicionarEmpresaReqDto
                {
                    DescricaoNomeFisica = usuario.Nome,
                    Telefone = usuario.Telefone
                };

                await _empresaServico.Cadastrar(empresaDto);
                var resultado = await _usuarioRepositorio.InserirUsuarioAsync(usuario);
                
                return _mapper.Map<AdicionarUsuarioResDto>(resultado);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao cadastrar usuário PetShop.", ex);
            }
        }

        /// <summary>
        /// Adiciona um novo usuário sem validações completas (uso interno).
        /// </summary>
        public async Task<int> AdicionarUsuarioNovo(AdicionarUsuarioReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de cadastro não podem ser nulos.");

            var usuario = _mapper.Map<Usuario>(dto);
            return await _usuarioRepositorio.InserirUsuarioAsync(usuario);
        }

        #endregion

        #region Buscar

        /// <summary>
        /// Busca usuários com base em critérios de filtro.
        /// </summary>
        public async Task<IEnumerable<BuscarUsuarioResDto>> Buscar(BuscarUsuarioReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Filtros de busca não podem ser nulos.");

            var usuarios = await _usuarioRepositorio.GetByParameters(dto);
            return _mapper.Map<IEnumerable<BuscarUsuarioResDto>>(usuarios);
        }

        #endregion

        #region Validações

        /// <summary>
        /// Valida se um telefone já está cadastrado.
        /// </summary>
        public async Task<bool> ValidarTelefoneCadastrado(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                throw new ArgumentException("Telefone não pode ser nulo ou vazio.", nameof(telefone));

            var usuario = await _usuarioRepositorio.BuscarPorTelefone(telefone);
            return usuario != null;
        }

        #endregion

        #region Autenticação

        /// <summary>
        /// Realiza login do usuário usando telefone ou CNPJ.
        /// </summary>
        public async Task<LoginResDto> Login(LoginReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Credenciais não podem ser nulas.");

            Usuario usuario = null;

            if (!string.IsNullOrEmpty(dto.Telefone))
            {
                usuario = await _usuarioRepositorio.BuscarPorTelefone(dto.Telefone);
            }
            else if (!string.IsNullOrEmpty(dto.CNPJ))
            {
                usuario = await _usuarioRepositorio.BuscarPorCnpj(dto.CNPJ);
            }

            if (usuario == null || usuario.Senha != dto.Senha)
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

            return GerarToken(usuario);
        }

        /// <summary>
        /// Gera token JWT para o usuário autenticado.
        /// </summary>
        public LoginResDto GerarToken(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario), "Usuário não pode ser nulo para gerar token.");

            var usuarioResDto = new BuscarUsuarioResDto
            {
                Id = usuario.Id,
                CNPJ = usuario.CNPJ,
                Nome = usuario.Nome,
                Senha = usuario.Senha,
                Email = usuario.Email,
                Telefone = usuario.Telefone
            };

            return new LoginResDto
            {
                LoginAtivado = true,
                BuscarUsuarioResDto = usuarioResDto
            };
        }

        #endregion

        #region Alterar Dados

        /// <summary>
        /// Altera a senha de um usuário.
        /// </summary>
        public async Task<bool> AlterarSenhaDoUsuario(AlterarSenhaReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de alteração não podem ser nulos.");

            ValidadorArquivo.ValidarId(dto.IdUsuario, "usuário");

            if (string.IsNullOrWhiteSpace(dto.SenhaNova))
                throw new ArgumentException("Senha nova não pode ser nula ou vazia.", nameof(dto.SenhaNova));

            return await _usuarioRepositorio.AlterarSenhaDoUsuario(dto.SenhaNova, dto.IdUsuario);
        }

        /// <summary>
        /// Altera dados pessoais de um usuário.
        /// </summary>
        public async Task<bool> AlterarUsuario(AlterarUsuarioReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de alteração não podem ser nulos.");

            ValidadorArquivo.ValidarId(dto.IdUsuario, "usuário");

            return await _usuarioRepositorio.AlterarUsuario(dto.Nome, dto.Telefone, dto.IdUsuario, dto.Email);
        }

        #endregion

        #region Excluir

        /// <summary>
        /// Exclui uma conta de usuário.
        /// </summary>
        public async Task<bool> Excluir(int idUsuario)
        {
            ValidadorArquivo.ValidarId(idUsuario, "usuário");
            return await _usuarioRepositorio.Excluir(idUsuario);
        }

        #endregion

        #region Gerenciar Foto de Perfil

        /// <summary>
        /// Lista todas as fotos de perfil de um usuário.
        /// </summary>
        public async Task<List<BuscarFotoUsuarioResDto>> ListarFotosDoUsuarios(int idUsuario)
        {
            ValidadorArquivo.ValidarId(idUsuario, "usuário");

            var urls = await _gerenciadorMidia.ListarMidiasAsync(idUsuario, "usuarios", "perfil");

            return urls
                .Select(url => new BuscarFotoUsuarioResDto { IdUsuario = idUsuario, Url = url })
                .ToList();
        }

        /// <summary>
        /// Realiza upload ou atualização da foto de perfil do usuário.
        /// </summary>
        public async Task<string> EnviarOuAtualizarFotoDoUsuario(IFormFile arquivo, int idUsuario)
        {
            ValidadorArquivo.ValidarId(idUsuario, "usuário");
            var extensao = ValidadorArquivo.ValidarEObtlerExtensao(arquivo, "foto de perfil");

            var chave = ValidadorArquivo.CriarChavenArquivo("usuarios", idUsuario, "perfil", extensao);

            using var stream = arquivo.OpenReadStream();
            return await _storageService.UploadAsync(chave, stream, arquivo.ContentType);
        }

        #endregion
    }
}