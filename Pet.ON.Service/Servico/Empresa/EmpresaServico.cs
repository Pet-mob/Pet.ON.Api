using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Pet.ON.Service.Servico
{
    /// <summary>
    /// Serviço para gerenciar operações de empresa.
    /// Responsabilidades: CRUD de empresa e horários de funcionamento.
    /// Armazenamento de mídia delegado para GerenciadorMidiaSobreEscrita.
    /// </summary>
    public class EmpresaServico : IEmpresaServico
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly GerenciadorMidiaSobreEscrita _gerenciadorMidia;

        public EmpresaServico(
            IEmpresaRepositorio empresaRepositorio, 
            IMapper mapper, 
            IStorageService storageService)
        {
            _empresaRepositorio = empresaRepositorio ?? throw new ArgumentNullException(nameof(empresaRepositorio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _gerenciadorMidia = new GerenciadorMidiaSobreEscrita(_storageService);
        }

        #region Atualizar

        /// <summary>
        /// Atualiza os dados de uma empresa.
        /// </summary>
        public async Task<AtualizarEmpresaResDto> Atualizar(AtualizarEmpresaReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de atualização não podem ser nulos.");

            try
            {
                var empresa = _mapper.Map<Empresa>(dto);
                var resultado = await _empresaRepositorio.UpdateAsync(empresa);
                return _mapper.Map<AtualizarEmpresaResDto>(resultado);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar empresa.", ex);
            }
        }

        /// <summary>
        /// Atualiza os horários de funcionamento de uma empresa.
        /// </summary>
        public async Task AtualizarHorariosFuncionamento(List<BuscarHorariosFuncionamentosEmpresaReqDto> dto)
        {
            if (dto == null || dto.Count == 0)
                throw new ArgumentException("Lista de horários não pode ser nula ou vazia.", nameof(dto));

            var horarios = _mapper.Map<List<HorariosFuncionamento>>(dto);
            await _empresaRepositorio.AtualizarHorariosFuncionamentoAsync(horarios);
        }

        #endregion

        #region Buscar

        /// <summary>
        /// Busca empresas com filtros aplicados e carrega suas logos e capas.
        /// </summary>
        public async Task<IEnumerable<BuscarEmpresaResDto>> Buscar(BuscarEmpresaReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Filtros de busca não podem ser nulos.");

            try
            {
                var empresas = await _empresaRepositorio.GetByParameters(dto);
                
                foreach (var empresa in empresas)
                {
                    await CarregarMidiasEmpresaAsync(empresa);
                }

                return _mapper.Map<IEnumerable<BuscarEmpresaResDto>>(empresas);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar empresas.", ex);
            }
        }

        /// <summary>
        /// Busca horários de funcionamento de uma empresa.
        /// </summary>
        public async Task<IEnumerable<BuscarHorariosFuncionamentosEmpresaResDto>> BuscarHorariosFuncionamento(
            BuscarHorariosFuncionamentosEmpresaReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Filtros de busca não podem ser nulos.");

            try
            {
                var horariosFuncionamentos = await _empresaRepositorio.BuscarHorariosFuncionamento(dto);
                return _mapper.Map<IEnumerable<BuscarHorariosFuncionamentosEmpresaResDto>>(horariosFuncionamentos);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar horários de funcionamento.", ex);
            }
        }

        #endregion

        #region Cadastrar

        /// <summary>
        /// Cadastra uma nova empresa.
        /// </summary>
        public async Task<AdicionarEmpresaResDto> Cadastrar(AdicionarEmpresaReqDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dados de cadastro não podem ser nulos.");

            try
            {
                var empresa = _mapper.Map<Empresa>(dto);
                var resultado = await _empresaRepositorio.InsertAsync(empresa);
                return _mapper.Map<AdicionarEmpresaResDto>(resultado);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao cadastrar empresa.", ex);
            }
        }

        #endregion

        #region Gerenciar Logos

        /// <summary>
        /// Lista todas as logos de uma empresa.
        /// </summary>
        public async Task<List<BuscarLogosResDto>> ListarLogosEmpresa(int idEmpresa)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");

            var urls = await _gerenciadorMidia.ListarMidiasAsync(idEmpresa, "empresas", "logo");

            return urls
                .Select(url => new BuscarLogosResDto { IdEmpresa = idEmpresa, Url = url })
                .ToList();
        }

        /// <summary>
        /// Realiza upload de uma nova logo de empresa.
        /// </summary>
        public async Task<string> EnviarLogoEmpresa(IFormFile arquivo, int idEmpresa)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");
            var extensao = ValidadorArquivo.ValidarEObtlerExtensao(arquivo, "logo de empresa");

            var chave = ValidadorArquivo.CriarChavenArquivo("empresas", idEmpresa, "logo", extensao);

            using var stream = arquivo.OpenReadStream();
            return await _storageService.UploadAsync(chave, stream, arquivo.ContentType);
        }

        #endregion

        #region Gerenciar Capas

        /// <summary>
        /// Lista todas as capas de uma empresa.
        /// </summary>
        public async Task<List<BuscarLogosResDto>> ListarCapaEmpresa(int idEmpresa)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");

            var urls = await _gerenciadorMidia.ListarMidiasAsync(idEmpresa, "empresas", "capa");

            return urls
                .Select(url => new BuscarLogosResDto { IdEmpresa = idEmpresa, Url = url })
                .ToList();
        }

        /// <summary>
        /// Realiza upload de uma nova capa de empresa.
        /// </summary>
        public async Task<string> EnviarCapaEmpresa(IFormFile arquivo, int idEmpresa)
        {
            ValidadorArquivo.ValidarId(idEmpresa, "empresa");
            var extensao = ValidadorArquivo.ValidarEObtlerExtensao(arquivo, "capa de empresa");

            var chave = ValidadorArquivo.CriarChavenArquivo("empresas", idEmpresa, "capa", extensao);

            using var stream = arquivo.OpenReadStream();
            return await _storageService.UploadAsync(chave, stream, arquivo.ContentType);
        }

        #endregion

        #region Buscar Empresas por Usuário

        /// <summary>
        /// Busca todas as empresas vinculadas a um usuário.
        /// </summary>
        public async Task<IEnumerable<BuscarEmpresaResDto>> BuscarEmpresasVinculadoAoUsuario(int idUsuario)
        {
            ValidadorArquivo.ValidarId(idUsuario, "usuário");

            var empresas = await _empresaRepositorio.BuscarEmpresasVinculadoAoUsuario(idUsuario);

            foreach (var empresa in empresas)
            {
                await CarregarMidiasEmpresaAsync(empresa);
            }

            return _mapper.Map<IEnumerable<BuscarEmpresaResDto>>(empresas);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Carrega logos e capas de uma empresa de forma segura.
        /// </summary>
        private async Task CarregarMidiasEmpresaAsync(Empresa empresa)
        {
            if (empresa == null) return;

            var urlLogo = await _gerenciadorMidia.ObterMidiaAsync(empresa.IdEmpresa, "empresas", "logo");
            if (!string.IsNullOrEmpty(urlLogo))
                empresa.UrlLogoEmpresa = urlLogo;

            var urlCapa = await _gerenciadorMidia.ObterMidiaAsync(empresa.IdEmpresa, "empresas", "capa");
            if (!string.IsNullOrEmpty(urlCapa))
                empresa.UrlCapaEmpresa = urlCapa;
        }

        #endregion
    }
}
