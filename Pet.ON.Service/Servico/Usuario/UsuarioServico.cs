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
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using Azure.Storage.Sas;
using System.Linq;

namespace Pet.ON.Service.Servico
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        private readonly IEmpresaServico _empresaServico;
        private readonly IStorageService _storageService;

        public UsuarioServico(
            IMapper mapper, 
            IEmpresaServico empresaServico, 
            IUsuarioRepositorio usuarioRepositorio,
            IStorageService storageService)
        {
            _mapper = mapper;
            _empresaServico = empresaServico;
            _usuarioRepositorio = usuarioRepositorio;
            _storageService = storageService;
        }

        public async Task Cadastrar(AdicionarUsuarioReqDto dto)
        {
            try
            {
                var usuario = _mapper.Map<Usuario>(dto);

                // Instanciar o validador de objeto
                var validador = new Validador<Usuario>();
                if (!validador.EstaValido(usuario))
                {
                    // Retornar notificações se o objeto não estiver válido
                    var notificacoes = validador.ObterNotificacoes();
                    // Aqui você pode manipular as notificações, por exemplo, enviá-las ao cliente
                    return;
                }

                // Inserir usuário no repositório
                var resultado = await _usuarioRepositorio.InserirUsuarioAsync(usuario);
            }
            catch (Exception)
            {
                throw;
                //criar log sobre o qual motivo do erro
                //mandar as notificação pro usuario.


                //_logger.LogError(ex, $@"Erro ao adicionar configuração de importação. Erro: {ex.Message} 
                //                 Parâmetro: {adicionarConfiguracaoImportacaoErp.ToJson()}");
                //_controleNotificacao.AdicionarNotificacao(LocalizacaoCaminho.ErroCadastrarConfiguracaoGeral);
            }

        }

        public async Task<IEnumerable<BuscarUsuarioResDto>> Buscar(BuscarUsuarioReqDto dto)
        {
            var usuarios = await _usuarioRepositorio.GetByParameters(dto);
            return _mapper.Map<IEnumerable<BuscarUsuarioResDto>>(usuarios);
        }

        public async Task<AdicionarUsuarioResDto> CadastrarUsuarioPetShop(AdicionarUsuarioReqDto dto)
        {
            try
            {
                var adicionar = _mapper.Map<Usuario>(dto);
                var empresaDto = new AdicionarEmpresaReqDto
                {
                    DescricaoNomeFisica = adicionar.Nome,
                    Telefone = adicionar.Telefone
                };

                var empresaAdicionado = _empresaServico.Cadastrar(empresaDto);                
                var resultado = await _usuarioRepositorio.InserirUsuarioAsync(adicionar);
                return _mapper.Map<AdicionarUsuarioResDto>(resultado);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LoginResDto> Login(LoginReqDto dto)
        {
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

            return GenerateToken(usuario);
        }

        public LoginResDto GenerateToken(Usuario usuario)
        {
            var buscarUsuarioResDto = new BuscarUsuarioResDto() 
            {
                Id = usuario.Id,
                CNPJ = usuario.CNPJ,
                Nome = usuario.Nome,
                Senha = usuario.Senha,
                Email = usuario.Email,
                Telefone = usuario.Telefone
            };

            //var secretKey = Environment.GetEnvironmentVariable("JwtSettings_SecretKey")
            //                ?? _configuration["JwtSettings:SecretKey"];

            //if (string.IsNullOrEmpty(secretKey))
            //    throw new InvalidOperationException("A chave secreta JWT não está configurada.");

            //var issuer = Environment.GetEnvironmentVariable("JwtSettings_Issuer")
            //             ?? _configuration["JwtSettings:Issuer"];

            //var audience = Environment.GetEnvironmentVariable("JwtSettings_Audience")
            //               ?? _configuration["JwtSettings:Audience"];

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, usuario.Nome),
            //    new Claim("Telefone", usuario.Telefone),
            //    new Claim(ClaimTypes.Role, "User")
            //};

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            //var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    issuer: issuer,
            //    audience: audience,
            //    claims: claims,
            //    expires: DateTime.UtcNow.AddHours(1),
            //    signingCredentials: credentials
            //);

            return new LoginResDto
            {
                //Token = new JwtSecurityTokenHandler().WriteToken(token),
                LoginAtivado = true,
                BuscarUsuarioResDto = buscarUsuarioResDto
            };
        }

        public async Task<bool> AlterarSenhaDoUsuario(AlterarSenhaReqDto dto)
        {
            var alterarSenhaDoUsuario = await _usuarioRepositorio.AlterarSenhaDoUsuario(dto.SenhaNova, dto.IdUsuario);
            return alterarSenhaDoUsuario;            
        }

        public async Task<bool> AlterarUsuario(AlterarUsuarioReqDto dto)
        {
            var alterarUsuario = await _usuarioRepositorio.AlterarUsuario(dto.Nome, dto.Telefone, dto.IdUsuario, dto.Email);
            return alterarUsuario;
        }
        
        public async Task<List<BuscarFotoUsuarioResDto>> ListarFotosDoUsuarios(int idUsuarioFiltro)
        {
            if (idUsuarioFiltro <= 0)
                return new List<BuscarFotoUsuarioResDto>();

            var prefix = $"usuarios/{idUsuarioFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            var fotos = urls
                .Where(url => url.Contains("/perfil."))
                .Select(url => new BuscarFotoUsuarioResDto
                {
                    IdUsuario = idUsuarioFiltro,
                    Url = url
                })
                .ToList();

            return fotos;
        }
        
        public async Task<string> EnviarOuAtualizarFotoDoUsuario(IFormFile arquivo, int idUsuario)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (idUsuario <= 0)
                throw new ArgumentException("Usuário inválido");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();

            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão inválida.");

            string key = $"usuarios/{idUsuario}/perfil{extensao}";

            using var stream = arquivo.OpenReadStream();

            return await _storageService.UploadAsync(
                key,
                stream,
                arquivo.ContentType
            );
        }
        
        public async Task<int> AdicionarUsuarioNovo(AdicionarUsuarioReqDto dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);
            var idGerado = await _usuarioRepositorio.InserirUsuarioAsync(usuario);
            return idGerado;
        }

        public async Task<bool> Excluir(int idUsuario)
        {
            var excluirConta = await _usuarioRepositorio.Excluir(idUsuario);
            return excluirConta;
        
        }

        public async Task<bool> ValidarTelefoneCadastrado(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
                throw new ArgumentException("Telefone inválido.");

            var usuario = await _usuarioRepositorio.BuscarPorTelefone(telefone);
            return usuario != null;
        }
    }
}