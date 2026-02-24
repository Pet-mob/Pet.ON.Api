using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Azure.Storage.Sas;

namespace Pet.ON.Service.Servico
{
    public class EmpresaServico : IEmpresaServico
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly IMapper _mapper;
        private readonly string _azureBlobConnection;

        public EmpresaServico(IEmpresaRepositorio empresaRepositorio, IMapper mapper, IConfiguration configuration)
        {
            _empresaRepositorio = empresaRepositorio;
            _mapper = mapper;
            _azureBlobConnection = configuration.GetConnectionString("AzureBlobConnection");
        }

        #region Atualizar
        public async Task<AtualizarEmpresaResDto> Atualizar(AtualizarEmpresaReqDto dto)
        {
            try
            {
                var empresa = _mapper.Map<Empresa>(dto);
                var resultado = await _empresaRepositorio.UpdateAsync(empresa);  // Usando repositório especializado
                return _mapper.Map<AtualizarEmpresaResDto>(resultado);
            }
            catch (Exception ex)
            {
                // Pode registrar o erro ou lançar uma exceção personalizada
                throw new Exception("Erro ao atualizar empresa", ex);
            }
        }

        public async Task AtualizarHorariosFuncionamento(List<BuscarHorariosFuncionamentosEmpresaReqDto> dto)
        {
            var horarios = _mapper.Map<List<HorariosFuncionamento>>(dto);
            await _empresaRepositorio.AtualizarHorariosFuncionamentoAsync(horarios);
        }
        #endregion

        #region Buscar
        public async Task<IEnumerable<BuscarEmpresaResDto>> Buscar(BuscarEmpresaReqDto dto)
        {
            try
            {
                var empresas = await _empresaRepositorio.GetByParameters(dto);
                foreach (var empresa in empresas)
                {
                    var logoEmpresa = await BuscarLogoEmpresa(empresa.IdEmpresa);
                    if (logoEmpresa.IdEmpresa ==  empresa.IdEmpresa)
                        empresa.UrlLogoEmpresa = logoEmpresa.Url;

                    var capaEmpresa = await BuscarCapaEmpresa(empresa.IdEmpresa);
                    if (capaEmpresa.IdEmpresa == empresa.IdEmpresa)
                        empresa.UrlCapaEmpresa = capaEmpresa.Url;

                }
                return _mapper.Map<IEnumerable<BuscarEmpresaResDto>>(empresas);                 
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar empresas", ex);
            }
        }

        public async Task<IEnumerable<BuscarHorariosFuncionamentosEmpresaResDto>> BuscarHorariosFuncionamento(BuscarHorariosFuncionamentosEmpresaReqDto dto)
        {
            try
            {
                var horariosFuncionamentos = await _empresaRepositorio.BuscarHorariosFuncionamento(dto);
                return _mapper.Map<IEnumerable<BuscarHorariosFuncionamentosEmpresaResDto>>(horariosFuncionamentos);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar horarios de funcionamento da empresas", ex);
            }
        }
        #endregion

        #region Cadastrar
        public async Task<AdicionarEmpresaResDto> Cadastrar(AdicionarEmpresaReqDto dto)
        {
            try
            {
                var empresa = _mapper.Map<Empresa>(dto);
                var resultado = await _empresaRepositorio.InsertAsync(empresa);  // Usando repositório especializado
                return _mapper.Map<AdicionarEmpresaResDto>(resultado);
            }
            catch (Exception ex)
            {
                // Pode registrar o erro ou lançar uma exceção personalizada
                throw new Exception("Erro ao cadastrar empresa", ex);
            }
        }

        public async Task<List<BuscarLogosResDto>> ListarCapaEmpresa(int idEmpresaFiltro)
        {

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "imagens";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new List<BuscarLogosResDto>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: empresa-123.jpg                                                 

                // Regex garante que extrai apenas arquivos que seguem o padrão "usuario_{id}.ext"
                var match = Regex.Match(nomeBlob, @"empresa_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int idEmpresaExtraido))
                {
                    if (idEmpresaFiltro == 0 || idEmpresaFiltro == idEmpresaExtraido)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.Add(new BuscarLogosResDto
                        {
                            IdEmpresa = idEmpresaExtraido,
                            Url = urlComSas
                        });
                    }
                }
            }

            return resultado;
        }

        private async Task<BuscarLogosResDto> BuscarLogoEmpresa(int idEmpresaFiltro)
        {

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "logos-empresas";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new BuscarLogosResDto();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: empresa-123.jpg                                                 

                // Regex garante que extrai apenas arquivos que seguem o padrão "usuario_{id}.ext"
                var match = Regex.Match(nomeBlob, @"empresa_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int idEmpresaExtraido))
                {
                    if (idEmpresaFiltro == 0 || idEmpresaFiltro == idEmpresaExtraido)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.IdEmpresa = idEmpresaExtraido;
                        resultado.Url = urlComSas;
                    }
                }
            }

            return resultado;
        }
        private async Task<BuscarLogosResDto> BuscarCapaEmpresa(int idEmpresaFiltro)
        {

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "imagens";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new BuscarLogosResDto();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: empresa-123.jpg                                                 

                // Regex garante que extrai apenas arquivos que seguem o padrão "usuario_{id}.ext"
                var match = Regex.Match(nomeBlob, @"empresa_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int idEmpresaExtraido))
                {
                    if (idEmpresaFiltro == 0 || idEmpresaFiltro == idEmpresaExtraido)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.IdEmpresa = idEmpresaExtraido;
                        resultado.Url = urlComSas;
                    }
                }
            }

            return resultado;
        }

        public async Task<List<BuscarLogosResDto>> ListarLogosEmpresa(int idEmpresaFiltro)
        {

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "logos-empresas";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new List<BuscarLogosResDto>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: empresa-123.jpg                                                 

                // Regex garante que extrai apenas arquivos que seguem o padrão "usuario_{id}.ext"
                var match = Regex.Match(nomeBlob, @"empresa_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int idEmpresaExtraido))
                {
                    if (idEmpresaFiltro == 0 || idEmpresaFiltro == idEmpresaExtraido)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.Add(new BuscarLogosResDto
                        {
                            IdEmpresa = idEmpresaExtraido,
                            Url = urlComSas
                        });
                    }
                }
            }

            return resultado;
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

        public async Task<string> EnviarLogoEmpresa(IFormFile arquivo, int idEmpresa)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();
            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão do arquivo inválida.");

            string nomeArquivo = $"empresa_{idEmpresa}{extensao}";
            string containerName = "logos-empresas";

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
        public async Task<string> EnviarCapaEmpresa(IFormFile arquivo, int idEmpresa)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();
            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão do arquivo inválida.");

            string nomeArquivo = $"empresa_{idEmpresa}{extensao}";
            string containerName = "imagens";

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

        public async Task<IEnumerable<BuscarEmpresaResDto>> BuscarEmpresasVinculadoAoUsuario(int idUsuario)
        {
            var empresas = await _empresaRepositorio.BuscarEmpresasVinculadoAoUsuario(idUsuario);
            foreach (var empresa in empresas)
            {
                var logoEmpresa = await BuscarLogoEmpresa(empresa.IdEmpresa);
                if (logoEmpresa.IdEmpresa == empresa.IdEmpresa)
                    empresa.UrlLogoEmpresa = logoEmpresa.Url;

                var capaEmpresa = await BuscarCapaEmpresa(empresa.IdEmpresa);
                if (capaEmpresa.IdEmpresa == empresa.IdEmpresa)
                    empresa.UrlCapaEmpresa = capaEmpresa.Url;

            }
            return _mapper.Map<IEnumerable<BuscarEmpresaResDto>>(empresas);
        }


        #endregion
    }
}
