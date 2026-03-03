using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    /// <summary>
    /// Interface para gerenciar operações de armazenamento em nuvem.
    /// Suporta AWS S3, Azure Blob, etc.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Realiza upload de um arquivo para o armazenamento em nuvem.
        /// </summary>
        /// <param name="key">Caminho/identificador único do arquivo</param>
        /// <param name="stream">Stream contendo o conteúdo do arquivo</param>
        /// <param name="contentType">MIME type do arquivo (ex: image/png)</param>
        /// <returns>URL pública de acesso ao arquivo</returns>
        /// <exception cref="ArgumentException">Se chave, fluxo ou tipoConteudo forem inválidos</exception>
        /// <exception cref="InvalidOperationException">Se houver erro na configuração da credencial</exception>
        Task<string> UploadAsync(string key, Stream stream, string contentType);
        
        /// <summary>
        /// Lista todos os arquivos com um prefixo específico no armazenamento.
        /// </summary>
        /// <param name="prefix">Prefixo para filtrar arquivos (ex: "usuarios/123/")</param>
        /// <returns>Lista de URLs públicas dos arquivos encontrados</returns>
        /// <exception cref="ArgumentException">Se prefixo for nulo ou vazio</exception>

        Task<List<string>> ListAsync(string prefix);
    }
}
