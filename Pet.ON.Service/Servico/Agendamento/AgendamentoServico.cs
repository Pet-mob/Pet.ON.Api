using AutoMapper;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Dtos.v1.Agendamento;
using Pet.ON.Domain.Dtos.v1.Animal;
using Pet.ON.Domain.Dtos.v1.Empresa;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Extensoes;
using Pet.ON.Domain.Interfaces.Hubs;
using Pet.ON.Domain.Interfaces.Repositorio;
using Pet.ON.Domain.Interfaces.Servico.v1;
using Pet.ON.Domain.Interfaces.Servico.v1.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Pet.ON.Domain.Dtos.v1.Agendamento.AgendaDiaResDto;

namespace Pet.ON.Service.Servico
{
    public class AgendamentoServico : IAgendamentoServico
    {
        private readonly IDbConnection _dbConnection;
        private readonly IAnimalServico _animalServico;
        private readonly IMapper _mapper;
        private readonly IAgendamentoRepositorio _agendamentoRepositorio;
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly INotificacaoDeAgendamento _notificador; 
        private readonly IParametrosServico _parametrosServico;

        public AgendamentoServico(
            IDbConnection dbConnection, 
            IMapper mapper,
            IAgendamentoRepositorio agendamentoRepositorio,
            IEmpresaRepositorio empresaRepositorio,
            IAnimalServico animalServico,
            INotificacaoDeAgendamento notificador,
            IParametrosServico parametrosServico)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;
            _agendamentoRepositorio = agendamentoRepositorio;
            _empresaRepositorio = empresaRepositorio;
            _animalServico = animalServico;
            _notificador = notificador;
            _parametrosServico = parametrosServico;
        }

        #region Metodos

        public async Task<List<BuscarAgendamentoResDto>> Buscar(BuscarAgendamentoReqDto dto)
        {
            try
            {
                var listaAgendamentos = await _agendamentoRepositorio.BuscarAgendamentos(dto.IdUsuario);
                var listarFotosAnimaisPorUsuario = await _animalServico.ListarFotosAnimaisPorUsuario(dto.IdUsuario) ?? new List<BuscarFotoAnimalResDto>();

                foreach (var agendamento in listaAgendamentos)
                {
                    var fotoAnimal = listarFotosAnimaisPorUsuario
                        .FirstOrDefault(f => f.IdUsuario == agendamento.IdUsuario && f.IdAnimal == agendamento.IdAnimal);

                    if (fotoAnimal != null)
                        agendamento.UrlFotoAnimal = fotoAnimal.Url;

                }

                return _mapper.Map<List<BuscarAgendamentoResDto>>(listaAgendamentos);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AdicionarAgendamentoResDto> Adicionar(AdicionarAgendamentoReqDto dto)
        {
            try
            {
                bool algumAgendamentoSalvo = false;

                foreach (var dataAgendamento in dto.ListaDatasAgendamento)
                {
                    int? idAgendamentoPai = null;
                    int contadorServicos = dto.IdServicos.Count();
                    int? idPrimeiroRegistro = null;

                    bool primeiro = true;

                    foreach (var idServico in dto.IdServicos)
                    {
                        var agendamento = new Agendamento
                        {
                            IdServico = idServico,
                            IdAnimal = dto.IdAnimal,
                            IdUsuario = dto.IdUsuario,
                            IdEmpresa = dto.IdEmpresa,
                            PacoteMensal = dto.PacoteMensal,
                            Data = dataAgendamento,
                            HorarioInicial = dto.Horario,
                            HorarioFinal = dto.HorarioFinal,
                            Status = dto.Status,
                            IdAgendamentoPai = idAgendamentoPai // null no primeiro
                        };

                        // Insere
                        int idGerado = await _agendamentoRepositorio.Adicionar(agendamento, primeiro);

                        if (primeiro)
                        {
                            idPrimeiroRegistro = idGerado;

                            // Se for apenas 1 serviço, o pai é ele mesmo
                            if (contadorServicos == 1)
                            {
                                await _agendamentoRepositorio.AtualizarIdAgendamentoPai(idGerado);
                            }

                            // Se forem vários serviços: agora estabelecemos o pai
                            idAgendamentoPai = idGerado;
                            primeiro = false;
                        }
                        else
                        {
                            // Filhos já recebem o id do primeiro
                        }

                        algumAgendamentoSalvo = true;
                        await _notificador.NotificarNovoAgendamentoAsync(dto.IdEmpresa, new { /* dados */ });
                    }

                    // Se houver múltiplos serviços, também precisamos atualizar o primeiro,
                    // para que o id_agendamento_pai dele seja igual ao próprio ID.
                    if (dto.IdServicos.Count() > 1 && idPrimeiroRegistro.HasValue)
                    {
                        await _agendamentoRepositorio.AtualizarIdAgendamentoPai(idPrimeiroRegistro.Value);
                    }
                }

                return new AdicionarAgendamentoResDto
                {
                    FoiAgendado = algumAgendamentoSalvo
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<BuscarHorariosDisponiveisResDto> BuscarHorariosDisponiveis(BuscarHorariosDisponiveisReqDto dto)
        {
            var resposta = new BuscarHorariosDisponiveisResDto();
            var horariosEmComum = new List<string>();

            // 1. Buscar parâmetros da empresa
            var parametros = await _parametrosServico.Buscar(dto.IdEmpresa);
            int limiteSimultaneo = parametros?.QtdeAtendimentoSimultaneoHorario ?? 1;

            foreach (var data in dto.ListaDataAgendamento)
            {
                var nomeDiaSemana = data.ToString("dddd", new CultureInfo("pt-BR"));
                var horarioFuncionamentoDto = new BuscarHorariosFuncionamentosEmpresaReqDto
                {
                    IdEmpresa = dto.IdEmpresa,
                    NomeDiaSemana = nomeDiaSemana
                };

                var funcionamento = await _empresaRepositorio.BuscarHorariosFuncionamento(horarioFuncionamentoDto);

                foreach (var horarioDiaFuncionamento in funcionamento)
                {
                    if (horarioDiaFuncionamento == null || !horarioDiaFuncionamento.FuncionaNesseDia)
                    {
                        return new BuscarHorariosDisponiveisResDto(); // se um dia não funcionar, retorna vazio
                    }

                    var agendamentos = await _agendamentoRepositorio.BuscarAgendamentosPorDia(dto.IdEmpresa, data);

                    var horariosDisponiveis = new List<string>();
                    var inicio = horarioDiaFuncionamento.HorarioAbertura;
                    var fim = horarioDiaFuncionamento.HorarioFechamento;
                    var duracao = TimeSpan.FromMinutes(dto.DuracaoEmMinutos);
                    var intervalo = TimeSpan.FromMinutes(horarioDiaFuncionamento.IntervaloEntreServicos);

                    while (inicio + duracao <= fim)
                    {
                        var fimAgendamento = inicio + duracao;

                        // Verifica se o horário atual já passou (apenas se a data for hoje)
                        var dataHoraInicio = data.Date + inicio;
                        var dataHoraAtual = DateTime.Now.Date + dto.HorarioAtual;
                        if (data.Date == DateTime.Today && dataHoraInicio < dataHoraAtual)
                        {
                            inicio += duracao + intervalo;
                            continue;
                        }

                        // Conta quantos agendamentos já existem nesse intervalo
                        var qtdeSimultaneo = agendamentos.Count(a =>
                            inicio < a.HorarioFinal && fimAgendamento > a.HorarioInicial
                        );

                        if (qtdeSimultaneo < limiteSimultaneo)
                        {
                            horariosDisponiveis.Add($"{inicio.Hours:D2}:{inicio.Minutes:D2}");
                        }

                        inicio += duracao + intervalo;
                    }

                    if (horariosEmComum.Count == 0)
                    {
                        horariosEmComum = horariosDisponiveis;
                    }
                    else
                    {
                        horariosEmComum = horariosEmComum.Intersect(horariosDisponiveis).ToList();
                    }

                    if (horariosEmComum.Count == 0)
                    {
                        return new BuscarHorariosDisponiveisResDto();
                    }
                }

                resposta.Horarios = horariosEmComum;
            }

            return resposta;
        }

        public async Task<DashboardAgendamentoResDto> Dashboard(DashboardAgendamentoReqDto dto)
        {
            var hoje = DateTime.Today;

            // Ajusta o início da semana para Segunda-feira
            int diasAteSegunda = ((int)hoje.DayOfWeek + 6) % 7; // ex: domingo = 0 vira 6
            var inicioSemana = hoje.AddDays(-diasAteSegunda);
            var fimSemana = inicioSemana.AddDays(6);

            // Busca dados principais e gráfico
            var dashboardBase = await _agendamentoRepositorio.DashboardAgendamento(dto.DataFiltro, dto.IdEmpresa);
            var dadosGrafico = await _agendamentoRepositorio.GraficoSemanal(inicioSemana, fimSemana, dto.IdEmpresa);

            // Mapeia o resultado e adiciona os dados do gráfico
            var resultado = _mapper.Map<DashboardAgendamentoResDto>(dashboardBase);
            resultado.GraficoSemanal = dadosGrafico;
            if (string.IsNullOrEmpty(resultado.ProximoHorario))
                resultado.ProximoHorario = "Nenhum agendamento";

            return resultado;
        }

        public async Task<List<AgendaDiaResDto>> Agenda(AgendaReqDto dto)
        {
            var listaAgendamentos = await _agendamentoRepositorio.BuscarAgenda(dto.DataFiltroInicio, dto.DataFiltroFim, dto.IdEmpresa);
            return _mapper.Map<List<AgendaDiaResDto>>(listaAgendamentos);
        }

        public async Task<int> BuscarQtdeAgendamentosDia(int idEmpresa, DateTime dataAgendamento, string horario)
        {
            return await _agendamentoRepositorio.BuscarQtdeAgendamentosDia(idEmpresa, dataAgendamento, horario);
        }

        public async Task<List<BuscarAgendamentoResDto>> BuscarAgendamentosPendentes(int idEmpresa)
        {
            try
            {
                var listaAgendamentos = await _agendamentoRepositorio.BuscarAgendamentosPendentes(idEmpresa);
                return _mapper.Map<List<BuscarAgendamentoResDto>>(listaAgendamentos);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AtualizarStatusAgendamento(int idAgendamento, int status)
        {
            if(status < 0)
                throw new ArgumentException("Status inválido.");
            var stringStatus = status == 1 ? "Concluido" : "Negado" ;
            return await _agendamentoRepositorio.AtualizarStatusAgendamento(idAgendamento, stringStatus);
        }

        #endregion
    }
}
