using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pet.ON.Service.Servico
{
    /// <summary>
    /// Serviço para validação de arquivos com regras de negócio comuns.
    /// Elimina duplicação de código entre EmpresaServico e UsuarioServico.
    /// </summary>
    public class ValidadorArquivo
    {
        /// <summary>
        /// Extensões de arquivo permitidas.
        /// </summary>
        private static readonly HashSet<string> ExtensoePermitidas = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        /// <summary>
        /// Tamanho máximo de arquivo em bytes (5 MB).
        /// </summary>
        private const long TamanoMaximoBytes = 5 * 1024 * 1024;

        /// <summary>
        /// Valida um arquivo e retorna a extensão em minúsculas.
        /// </summary>
        /// <param name="arquivo">Arquivo a validar</param>
        /// <param name="nomeEntidade">Nome da entidade para mensagem de erro (ex: "usuário", "empresa")</param>
        /// <returns>Extensão do arquivo em minúsculas (ex: ".png")</returns>
        /// <exception cref="ArgumentException">Se arquivo for inválido</exception>
        public static string ValidarEObtlerExtensao(IFormFile arquivo, string nomeEntidade = "arquivo")
        {
            if (arquivo == null)
                throw new ArgumentException($"Arquivo inválido para {nomeEntidade}.", nameof(arquivo));

            if (arquivo.Length == 0)
                throw new ArgumentException($"Arquivo vazio para {nomeEntidade}.", nameof(arquivo));

            if (arquivo.Length > TamanoMaximoBytes)
                throw new ArgumentException($"Arquivo para {nomeEntidade} excede tamanho máximo de 5 MB.", nameof(arquivo));

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();

            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException($"Extensão inválida para {nomeEntidade}.", nameof(arquivo));

            if (!ExtensoePermitidas.Contains(extensao))
                throw new ArgumentException(
                    $"Tipo de arquivo não suportado para {nomeEntidade}. Extensões permitidas: {string.Join(", ", ExtensoePermitidas)}",
                    nameof(arquivo));

            return extensao;
        }

        /// <summary>
        /// Valida um ID de entidade.
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <param name="nomeEntidade">Nome da entidade para mensagem de erro</param>
        /// <exception cref="ArgumentException">Se ID for inválido</exception>
        public static void ValidarId(int id, string nomeEntidade = "entidade")
        {
            if (id <= 0)
                throw new ArgumentException($"{nomeEntidade} inválida. ID deve ser maior que zero.", nameof(id));
        }

        /// <summary>
        /// Cria uma chave de arquivo padronizada.
        /// </summary>
        /// <param name="caminho">Caminho base (ex: "usuarios", "empresas")</param>
        /// <param name="id">ID da entidade</param>
        /// <param name="tipoArquivo">Tipo de arquivo (ex: "perfil", "logo", "capa")</param>
        /// <param name="extensao">Extensão do arquivo</param>
        /// <returns>Chave formatada (ex: "usuarios/123/perfil.png")</returns>
        public static string CriarChavenArquivo(string caminho, int id, string tipoArquivo, string extensao)
        {
            if (string.IsNullOrWhiteSpace(caminho))
                throw new ArgumentException("Caminho não pode ser nulo ou vazio.", nameof(caminho));

            if (string.IsNullOrWhiteSpace(tipoArquivo))
                throw new ArgumentException("Tipo de arquivo não pode ser nulo ou vazio.", nameof(tipoArquivo));

            ValidarId(id, "entidade");

            return $"{caminho.TrimEnd('/')}/{id}/{tipoArquivo}{extensao}";
        }
    }
}
