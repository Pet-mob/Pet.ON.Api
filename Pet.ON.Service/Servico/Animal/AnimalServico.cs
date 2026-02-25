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
using System.Linq;

namespace Pet.ON.Service.Servico
{
    public class AnimalServico : IAnimalServico
    {
        private readonly IMapper _mapper;
        private readonly IAnimalRepositorio _animalRepositorio;
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storageService;

        public AnimalServico(IAnimalRepositorio animalRepositorio, IMapper mapper, IConfiguration configuration, IStorageService storageService)
        {
            _animalRepositorio = animalRepositorio;
            _mapper = mapper;
            _configuration = configuration;
            _storageService = storageService;
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
                if (fotoAnimal == null)
                    continue;
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
            if (idUsuarioFiltro <= 0)
                return null;

            var prefix = $"animais/usuario_{idUsuarioFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            if (urls == null || !urls.Any())
                return null;

            // Pega a primeira foto encontrada
            var url = urls.First();

            var fileName = Path.GetFileNameWithoutExtension(url);

            var match = Regex.Match(fileName, @"animal_(\d+)", RegexOptions.IgnoreCase);

            int idAnimal = 0;

            if (match.Success)
                int.TryParse(match.Groups[1].Value, out idAnimal);

            return new BuscarFotoAnimalResDto
            {
                IdUsuario = idUsuarioFiltro,
                IdAnimal = idAnimal,
                Url = url
            };
        }

        public async Task<List<BuscarFotoAnimalResDto>> ListarFotosAnimaisPorUsuario(int idUsuario)
        {
            var prefix = $"animais/usuario_{idUsuario}/";

            var urls = await _storageService.ListAsync(prefix);

            return urls.Select(url =>
            {
                var fileName = Path.GetFileNameWithoutExtension(url);
                var match = Regex.Match(fileName, @"animal_(\d+)");

                return new BuscarFotoAnimalResDto
                {
                    IdUsuario = idUsuario,
                    IdAnimal = match.Success ? int.Parse(match.Groups[1].Value) : 0,
                    Url = url
                };
            }).ToList();
        }

        public async Task<string> EnviarOuAtualizarFotoDoAnimal(IFormFile arquivo, int idUsuario, int idAnimal)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (idUsuario <= 0 || idAnimal <= 0)
                throw new ArgumentException("Usuário ou Animal inválido");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();

            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão do arquivo inválida.");

            // 🔥 Novo padrão organizado
            string key = $"animais/usuario_{idUsuario}/animal_{idAnimal}{extensao}";

            using (var stream = arquivo.OpenReadStream())
            {
                var url = await _storageService.UploadAsync(
                    key,
                    stream,
                    arquivo.ContentType
                );

                return url;
            }
        }
        
        public async Task<int> AdcionarAnimalNovo(AdicionarAnimalReqDto dto)
        {

            var animal = _mapper.Map<Animal>(dto);
            var resultado = await _animalRepositorio.AdicionarAnimalNovo(animal);
            return resultado;
        }

    }
}
