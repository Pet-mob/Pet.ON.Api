using Pet.ON.Domain.Dtos.v1.Empresa;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pet.ON.Service.Servico
{
    /// <summary>
    /// Serviço genérico para gerenciar busca de arquivos de mídia.
    /// Elimina duplicação de código em métodos que buscam logos, capas, fotos, etc.
    /// </summary>
    public class GerenciadorMidiaSobreEscrita
    {
        private readonly IStorageService _storageService;

        public GerenciadorMidiaSobreEscrita(IStorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        /// <summary>
        /// Obtém uma única mídia com filtro específico.
        /// </summary>
        /// <param name="id">ID da entidade</param>
        /// <param name="caminho">Caminho base no storage (ex: "usuarios", "empresas")</param>
        /// <param name="nomeArquivo">Nome do arquivo a buscar (ex: "perfil", "logo")</param>
        /// <returns>URL da mídia ou null se não encontrada</returns>
        public async Task<string> ObterMidiaAsync(int id, string caminho, string nomeArquivo)
        {
            if (id <= 0) return null;
            if (string.IsNullOrWhiteSpace(caminho) || string.IsNullOrWhiteSpace(nomeArquivo)) return null;

            var prefixo = $"{caminho.TrimEnd('/')}/{id}/";
            var urls = await _storageService.ListAsync(prefixo);

            return urls.FirstOrDefault(url => url.Contains($"/{nomeArquivo}."));
        }

        /// <summary>
        /// Obtém múltiplas mídias com filtro específico.
        /// </summary>
        /// <param name="id">ID da entidade</param>
        /// <param name="caminho">Caminho base no storage</param>
        /// <param name="nomeArquivo">Nome do arquivo a filtrar</param>
        /// <returns>Lista de URLs encontradas</returns>
        public async Task<List<string>> ListarMidiasAsync(int id, string caminho, string nomeArquivo)
        {
            if (id <= 0) return new List<string>();
            if (string.IsNullOrWhiteSpace(caminho) || string.IsNullOrWhiteSpace(nomeArquivo))
                return new List<string>();

            var prefixo = $"{caminho.TrimEnd('/')}/{id}/";
            var urls = await _storageService.ListAsync(prefixo);

            return urls
                .Where(url => url.Contains($"/{nomeArquivo}."))
                .ToList();
        }
    }
}
