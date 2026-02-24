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

namespace Pet.ON.Service.Servico
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        private readonly IEmpresaServico _empresaServico;
        private readonly IConfiguration _configuration;
        private readonly string _azureBlobConnection;

        public UsuarioServico(
            IMapper mapper, 
            IEmpresaServico empresaServico, 
            IUsuarioRepositorio usuarioRepositorio,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _empresaServico = empresaServico;
            _usuarioRepositorio = usuarioRepositorio;
            _configuration = configuration;
            _azureBlobConnection = configuration.GetConnectionString("AzureBlobConnection");
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
            catch (Exception ex)
            {
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
            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "fotos-usuarios";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new List<BuscarFotoUsuarioResDto>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: usuario_123.jpg

                // Regex garante que extrai apenas arquivos que seguem o padrão "usuario_{id}.ext"
                var match = Regex.Match(nomeBlob, @"usuario_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int idUsuarioExtraido))
                {
                    if (idUsuarioFiltro == 0 || idUsuarioFiltro == idUsuarioExtraido)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.Add(new BuscarFotoUsuarioResDto
                        {
                            IdUsuario = idUsuarioExtraido,
                            Url = urlComSas
                        });
                    }
                }
            }

            return resultado;
        }

        public async Task<string> EnviarOuAtualizarFotoDoUsuario(IFormFile arquivo, int idUsuario)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();
            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão do arquivo inválida.");

            string nomeArquivo = $"usuario_{idUsuario}{extensao}";
            string containerName = "fotos-usuarios";

            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Cria o container se ainda não existir
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(nomeArquivo);

            // Envia e sobrescreve se já existir
            using (var stream = arquivo.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Retorna URL com SAS
            return ObterUrlComSas(blobClient);
        }

        public string ObterUrlComSas(BlobClient blobClient)
        {
            if (!blobClient.CanGenerateSasUri)
                throw new InvalidOperationException("BlobClient não pode gerar SAS URI. Verifique se a chave foi criada corretamente.");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
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