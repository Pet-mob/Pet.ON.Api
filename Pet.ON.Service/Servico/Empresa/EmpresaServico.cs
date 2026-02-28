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
using System.Linq;

namespace Pet.ON.Service.Servico
{
    public class EmpresaServico : IEmpresaServico
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;

        public EmpresaServico(IEmpresaRepositorio empresaRepositorio, IMapper mapper, IConfiguration configuration, IStorageService storageService)
        {
            _empresaRepositorio = empresaRepositorio;
            _mapper = mapper;
            _storageService = storageService;
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
                    if (logoEmpresa != null && logoEmpresa.IdEmpresa == empresa.IdEmpresa)
                        empresa.UrlLogoEmpresa = logoEmpresa.Url;

                    var capaEmpresa = await BuscarCapaEmpresa(empresa.IdEmpresa);
                    if (capaEmpresa != null && capaEmpresa.IdEmpresa == empresa.IdEmpresa)
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
            if (idEmpresaFiltro <= 0)
                return new List<BuscarLogosResDto>();

            var prefix = $"empresas/{idEmpresaFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            var capas = urls
                .Where(url => url.Contains("/capa."))
                .Select(url => new BuscarLogosResDto
                {
                    IdEmpresa = idEmpresaFiltro,
                    Url = url
                })
                .ToList();

            return capas;
        }

        private async Task<BuscarLogosResDto> BuscarLogoEmpresa(int idEmpresaFiltro)
        {
            if (idEmpresaFiltro <= 0)
                return null;

            var prefix = $"empresas/{idEmpresaFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            var logoUrl = urls.FirstOrDefault(u => u.Contains("/logo."));

            if (logoUrl == null)
                return null;

            return new BuscarLogosResDto
            {
                IdEmpresa = idEmpresaFiltro,
                Url = logoUrl
            };
        }

        private async Task<BuscarLogosResDto> BuscarCapaEmpresa(int idEmpresaFiltro)
        {
            if (idEmpresaFiltro <= 0)
                return null;

            var prefix = $"empresas/{idEmpresaFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            var capaUrl = urls.FirstOrDefault(u => u.Contains("/capa."));

            if (capaUrl == null)
                return null;

            return new BuscarLogosResDto
            {
                IdEmpresa = idEmpresaFiltro,
                Url = capaUrl
            };
        }
        public async Task<List<BuscarLogosResDto>> ListarLogosEmpresa(int idEmpresaFiltro)
        {
            if (idEmpresaFiltro <= 0)
                return new List<BuscarLogosResDto>();

            var prefix = $"empresas/{idEmpresaFiltro}/";

            var urls = await _storageService.ListAsync(prefix);

            var logos = urls
                .Where(url => url.Contains("/logo."))
                .Select(url => new BuscarLogosResDto
                {
                    IdEmpresa = idEmpresaFiltro,
                    Url = url
                })
                .ToList();

            return logos;
        }
        public async Task<string> EnviarLogoEmpresa(IFormFile arquivo, int idEmpresa)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (idEmpresa <= 0)
                throw new ArgumentException("Empresa inválida");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();

            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão inválida.");

            string key = $"empresas/{idEmpresa}/logo{extensao}";

            using var stream = arquivo.OpenReadStream();

            return await _storageService.UploadAsync(
                key,
                stream,
                arquivo.ContentType
            );
        }

        public async Task<string> EnviarCapaEmpresa(IFormFile arquivo, int idEmpresa)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");

            if (idEmpresa <= 0)
                throw new ArgumentException("Empresa inválida");

            string extensao = Path.GetExtension(arquivo.FileName)?.ToLower();

            if (string.IsNullOrEmpty(extensao) || !extensao.StartsWith("."))
                throw new ArgumentException("Extensão inválida.");

            string key = $"empresas/{idEmpresa}/capa{extensao}";

            using var stream = arquivo.OpenReadStream();

            return await _storageService.UploadAsync(
                key,
                stream,
                arquivo.ContentType
            );
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
