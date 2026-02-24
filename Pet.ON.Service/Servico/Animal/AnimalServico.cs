using AutoMapper;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Animal;
using Pet.ON.Domain.Dtos.v1.Usuario;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Sas;

namespace Pet.ON.Service.Servico
{
    public class AnimalServico : IAnimalServico
    {
        private readonly IMapper _mapper;
        private readonly IAnimalRepositorio _animalRepositorio;
        private readonly IConfiguration _configuration;
        private readonly string _azureBlobConnection;

        public AnimalServico(IAnimalRepositorio animalRepositorio, IMapper mapper, IConfiguration configuration)
        {
            _animalRepositorio = animalRepositorio;
            _mapper = mapper;
            _configuration = configuration;
            _azureBlobConnection = configuration.GetConnectionString("AzureBlobConnection");
        }

        public async Task<bool> Adicionar(AdicionarAnimalReqDto dto)
        {

            var animal = _mapper.Map<Animal>(dto);
            var resultado = await _animalRepositorio.Adicionar(animal);
            return resultado;
        }

        public async Task<bool> Alterar(AlterarAnimalReqDto dto)
        {
            var alterarAnimal = await _animalRepositorio.AlterarAnimal(dto.Nome, dto.Idade, dto.Raca, dto.Observacoes, dto.IdAnimal, dto.IdUsuario, dto.IdPorte);
            return alterarAnimal;
        }

        public async Task<IEnumerable<BuscarAnimalResDto>> Buscar(BuscarAnimalReqDto dto)
        {
            var animais = await _animalRepositorio.GetByParameters(dto);
            foreach (var animal in animais)
            {
                var fotoAnimal = await BuscarFotoAnimalPorUsuario(animal.IdUsuario);
                if (fotoAnimal.IdUsuario == animal.IdUsuario)
                    animal.UrlFotoAnimal = fotoAnimal.Url;
            }

            return _mapper.Map<IEnumerable<BuscarAnimalResDto>>(animais);
        }

        public async Task<bool> Excluir(int idUsuario, int idAnimal)
        {            
            var resultado = await _animalRepositorio.Excluir(idUsuario, idAnimal);
            return resultado;
        }

        public async Task<BuscarFotoAnimalResDto> BuscarFotoAnimalPorUsuario(int idUsuarioFiltro)
        {
            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "fotos-animais";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new BuscarFotoAnimalResDto();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: usuario_5_animal_12.jpg
                string url = $"{containerClient.Uri}/{nomeBlob}";

                // Regex para capturar idUsuario e idAnimal do nome do blob
                var match = Regex.Match(nomeBlob, @"usuario_(\d+)_animal_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success
                    && int.TryParse(match.Groups[1].Value, out int idUsuarioDoBlob)
                    && int.TryParse(match.Groups[2].Value, out int idAnimal))
                {
                    if (idUsuarioFiltro == 0 || idUsuarioFiltro == idUsuarioDoBlob)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado = new BuscarFotoAnimalResDto
                        {
                            IdUsuario = idUsuarioDoBlob,
                            IdAnimal = idAnimal,
                            Url = urlComSas
                        };
                    }
                }

            }

            return resultado;
        }


        public async Task<List<BuscarFotoAnimalResDto>> ListarFotosAnimaisPorUsuario(int idUsuarioFiltro)
        {
            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string containerName = "fotos-animais";
            var blobServiceClient = new BlobServiceClient(_azureBlobConnection);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var resultado = new List<BuscarFotoAnimalResDto>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                string nomeBlob = blobItem.Name; // Ex: usuario_5_animal_12.jpg
                string url = $"{containerClient.Uri}/{nomeBlob}";

                // Regex para capturar idUsuario e idAnimal do nome do blob
                var match = Regex.Match(nomeBlob, @"usuario_(\d+)_animal_(\d+)", RegexOptions.IgnoreCase);
                if (match.Success
                    && int.TryParse(match.Groups[1].Value, out int idUsuarioDoBlob)
                    && int.TryParse(match.Groups[2].Value, out int idAnimal))
                {
                    if (idUsuarioFiltro == 0 || idUsuarioFiltro == idUsuarioDoBlob)
                    {
                        var blobClient = containerClient.GetBlobClient(nomeBlob);
                        string urlComSas = ObterUrlComSas(blobClient);

                        resultado.Add(new BuscarFotoAnimalResDto
                        {
                            IdUsuario = idUsuarioDoBlob,
                            IdAnimal = idAnimal,
                            Url = urlComSas
                        });
                    }
                }

            }

            return resultado;
        }

        public async Task<string> EnviarOuAtualizarFotoDoAnimal(IFormFile arquivo, int idUsuario, int idAnimal)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (string.IsNullOrEmpty(_azureBlobConnection))
                throw new InvalidOperationException("A string de conexão com o Azure Blob não foi configurada.");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();
            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão do arquivo inválida.");

            string nomeArquivo = $"usuario_{idUsuario}_animal_{idAnimal}{extensao}";
            string containerName = "fotos-animais";

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
        
        public async Task<int> AdcionarAnimalNovo(AdicionarAnimalReqDto dto)
        {

            var animal = _mapper.Map<Animal>(dto);
            var resultado = await _animalRepositorio.AdicionarAnimalNovo(animal);
            return resultado;
        }

    }
}
