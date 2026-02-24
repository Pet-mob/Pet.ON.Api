using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using System.Collections.Generic;
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Pet.ON.Domain.Dtos.v1.Agendamento;
using static Pet.ON.Domain.Dtos.v1.Agendamento.AgendaDiaResDto;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;

namespace Pet.ON.Infra.Repositorio
{
    public class AgendamentoRepositorio : IAgendamentoRepositorio
    {
        private readonly IDbConnection _dbConnection;  // Usando conexão Dapper

        public AgendamentoRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> Adicionar(Agendamento agendamento, bool primeiro)
        {
            var sql = @"INSERT INTO Agendamentos 
                (pacote_mensal, id_servico, data, horario_inicial, horario_final, 
                 id_animal, id_usuario, id_empresa, status, id_agendamento_pai)
                OUTPUT INSERTED.id_agendamento
                VALUES 
                (@PacoteMensal, @IdServico, @Data, @HorarioInicial, 
                 @HorarioFinal, @IdAnimal, @IdUsuario, @IdEmpresa, @Status,
                 @IdAgendamentoPai)";

            var id = await _dbConnection.ExecuteScalarAsync<int>(sql, agendamento);
            return id;
        }

        public async Task AtualizarIdAgendamentoPai(int id)
        {
            var sql = @"UPDATE Agendamentos 
                SET id_agendamento_pai = @Id
                WHERE id_agendamento = @Id";

            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgendamentos(int idUsuario)
        {
            var sql = @"
                SELECT 
                    MIN(a.id_agendamento) AS IdAgendamento, 
                    a.pacote_mensal AS PacoteMensal, 
                    STRING_AGG(a.id_servico, ',') AS IdServicos,
                    STRING_AGG(s.nome_servico, ', ') AS DescricaoServicos,
                    a.data AS Data, 
                    a.horario_inicial AS HorarioInicial,
                    a.horario_final AS HorarioFinal,
                    a.id_animal AS IdAnimal,
                    an.nome AS NomeAnimal,
                    a.id_usuario AS IdUsuario,
                    u.nome AS NomeUsuario,
                    a.id_empresa AS IdEmpresa,
                    e.fantasia_razao_social AS NomeEmpresa,
                    a.status AS Status
                FROM Agendamentos a
                JOIN Animal an on an.id_animal = a.id_animal
                JOIN Servicos s on s.id_servico = a.id_servico
                JOIN Empresa e on e.id_empresa = a.id_empresa
                JOIN Usuario u on u.id = a.id_usuario
                WHERE a.id_usuario = @IdUsuario
                GROUP BY 
                    a.pacote_mensal, a.data, a.horario_inicial, a.horario_final, a.id_animal, an.nome,
                    a.id_usuario, u.nome, a.id_empresa, e.fantasia_razao_social, a.status
                ORDER BY a.data DESC
            ";

            return await _dbConnection.QueryAsync<AgendamentoCamposAuxiliares>(sql, new
            {
                IdUsuario = idUsuario
            });
        }

        public async Task<IEnumerable<Agendamento>> BuscarAgendamentosPorDia(int idEmpresa, DateTime dataAgendamento)
        {
            var sql = @"
                SELECT 
                    id_agendamento AS IdAgendamento, 
                    pacote_mensal AS PacoteMensal, 
                    id_servico AS IdServico, 
                    data AS Data, 
                    horario_inicial AS HorarioInicial,
                    horario_final AS HorarioFinal,
                    id_animal AS IdAnimal,
                    id_usuario AS IdUsuario,
                    id_empresa AS IdEmpresa,
                    status AS Status
                FROM Agendamentos
                WHERE id_empresa = @IdEmpresa AND CAST(Data AS DATE) = @Data
            ";

            return await _dbConnection.QueryAsync<Agendamento>(sql, new
            {
                IdEmpresa = idEmpresa,
                Data = dataAgendamento.Date
            });

            
        }

        public async Task<DashboardAgendamentos> DashboardAgendamento(DateTime dataFiltro, int idEmpresa)
        {
            var dataHoje = dataFiltro.Date;
            var dataAmanha = dataHoje.AddDays(1);

            const string query = @"
            SELECT
                (SELECT COUNT(1) 
                 FROM Agendamentos 
                 WHERE CAST(Data AS DATE) = @dataHoje AND id_empresa = @idEmpresa) AS PetsAgendadosHoje,

                (SELECT COUNT(1) 
                 FROM Agendamentos 
                 WHERE CAST(Data AS DATE) = @dataHoje AND Status = 'Concluido' AND id_empresa = @idEmpresa) AS ServicosConcluidosHoje,

                (SELECT COUNT(1) 
                 FROM Agendamentos 
                 WHERE CAST(Data AS DATE) = @dataAmanha AND id_empresa = @idEmpresa) AS AgendamentosAmanha,

                (
                    SELECT ISNULL(descricao_agendamento, 'Nenhum próximo agendamento') 
                    FROM (
                        SELECT TOP 1 
                            FORMAT(CAST(a.data AS DATETIME) + CAST(a.horario_inicial AS DATETIME), 'HH:mm') 
                            + ' - ' + s.nome_servico + ' (' + an.nome + ')' AS descricao_agendamento
                        FROM agendamentos a
                        INNER JOIN servicos s ON a.id_servico = s.id_servico AND s.id_empresa = a.id_empresa
                        INNER JOIN animal an ON a.id_animal = an.id_animal
                        WHERE CAST(a.data AS DATETIME) + CAST(a.horario_inicial AS DATETIME) > GETDATE() 
                          AND Status = 'Agendado' AND a.id_empresa = @idEmpresa
                        ORDER BY a.data, a.horario_inicial
                    ) AS Sub
                ) AS ProximoHorario;
            ";

            return await _dbConnection.QueryFirstAsync<DashboardAgendamentos>(query, new
            {
                dataHoje,
                dataAmanha,
                idEmpresa
            });
        }

        public async Task<int[]> GraficoSemanal(DateTime inicioSemana, DateTime fimSemana, int idEmpresa)
        {        

            var resultados = await _dbConnection.QueryAsync<(int DiaSemanaNumero, int Total)>(
                @"SELECT 
                    DATEPART(WEEKDAY, data) AS DiaSemanaNumero, 
                    COUNT(*) AS Total
                  FROM agendamentos
                  WHERE id_empresa = @idEmpresa AND 
                        data BETWEEN @inicioSemana AND @fimSemana
                    GROUP BY DATEPART(WEEKDAY, data)",
                new { idEmpresa, inicioSemana, fimSemana });

            // Inicia o array com 0 para os 7 dias da semana
            var dadosSemana = new int[7];

            foreach (var item in resultados)
            {
                // Ajusta índice: Segunda = 0, Domingo = 6
                // SQL Server padrão: 1=Domingo, 2=Segunda, ..., 7=Sábado
                int indice = item.DiaSemanaNumero == 1 ? 6 : item.DiaSemanaNumero - 2;
                dadosSemana[indice] = item.Total;
            }

            return dadosSemana;
        }       

        public async Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgenda(DateTime? dataFiltroInicio, DateTime? dataFiltroFim, int? idEmpresa)
        {
            var sql = @"
                SELECT 
                    MIN(a.id_agendamento) AS IdAgendamento, 
                    a.pacote_mensal AS PacoteMensal, 
                    STRING_AGG(a.id_servico, ',') AS IdServicos,
                    STRING_AGG(s.nome_servico, ', ') AS DescricaoServicos,
                    a.data AS Data, 
                    a.horario_inicial AS HorarioInicial,
                    a.horario_final AS HorarioFinal,
                    a.id_animal AS IdAnimal,
                    an.nome AS NomeAnimal,
                    a.id_usuario AS IdUsuario,
                    u.nome AS NomeUsuario,
                    a.id_empresa AS IdEmpresa,
                    e.fantasia_razao_social AS NomeEmpresa,
                    a.status AS Status
                FROM Agendamentos a
                JOIN Animal an on an.id_animal = a.id_animal
                JOIN Servicos s on s.id_servico = a.id_servico
                JOIN Empresa e on e.id_empresa = a.id_empresa
                JOIN Usuario u on u.id = a.id_usuario
                WHERE a.status = 'Concluido'                
            ";

            if (idEmpresa > 0)
                sql += " AND a.id_empresa = @idEmpresa ";

            if (dataFiltroInicio.HasValue && dataFiltroFim.HasValue)
                if (dataFiltroInicio <= dataFiltroFim && dataFiltroFim >= dataFiltroInicio)
                    sql += " AND a.data BETWEEN @dataFiltroInicio AND @dataFiltroFim ";

            sql += @"
                     GROUP BY 
                        a.pacote_mensal, a.data, a.horario_inicial, a.horario_final, a.id_animal, an.nome,
                        a.id_usuario, u.nome, a.id_empresa, e.fantasia_razao_social, a.status 
                    ORDER BY a.data Desc ";

            return await _dbConnection.QueryAsync<AgendamentoCamposAuxiliares>(sql, new
            {
                dataFiltroInicio,
                dataFiltroFim,
                idEmpresa
            });
        }

        public async Task<int> BuscarQtdeAgendamentosDia(int idEmpresa, DateTime dataAgendamento, string horario)
        {
            var sql = @"
                SELECT 
                    COUNT(1)                    
                FROM Agendamentos
                WHERE id_empresa = @IdEmpresa AND CAST(Data AS DATE) = @Data and status = 'Agendado' AND 
                      CAST(Horario_Inicial AS TIME) = @Horario
            ";

            return await _dbConnection.QueryFirstAsync<int>(sql, new
            {
                IdEmpresa = idEmpresa,
                Data = dataAgendamento.Date,
                Horario = horario
            });


        }

        public async Task<IEnumerable<AgendamentoCamposAuxiliares>> BuscarAgendamentosPendentes(int idEmpresa)
        {
            var sql = @"
                SELECT 
                    MIN(a.id_agendamento) AS IdAgendamento, 
                    a.pacote_mensal AS PacoteMensal, 
                    STRING_AGG(a.id_servico, ',') AS IdServicos,
                    STRING_AGG(s.nome_servico, ', ') AS DescricaoServicos,
                    a.data AS Data, 
                    a.horario_inicial AS HorarioInicial,
                    a.horario_final AS HorarioFinal,
                    a.id_animal AS IdAnimal,
                    an.nome AS NomeAnimal,
                    a.id_usuario AS IdUsuario,
                    u.nome AS NomeUsuario,
                    a.id_empresa AS IdEmpresa,
                    e.fantasia_razao_social AS NomeEmpresa,
                    a.status AS Status
                FROM Agendamentos a
                JOIN Animal an on an.id_animal = a.id_animal
                JOIN Servicos s on s.id_servico = a.id_servico
                JOIN Empresa e on e.id_empresa = a.id_empresa
                JOIN Usuario u on u.id = a.id_usuario
                WHERE a.id_empresa = @IdEmpresa AND a.status = 'Agendado'
                GROUP BY 
                    a.pacote_mensal, a.data, a.horario_inicial, a.horario_final, a.id_animal, an.nome,
                    a.id_usuario, u.nome, a.id_empresa, e.fantasia_razao_social, a.status
                ORDER BY a.data DESC
            ";

            return await _dbConnection.QueryAsync<AgendamentoCamposAuxiliares>(sql, new
            {
                IdEmpresa = idEmpresa
            });
        }

        public Task<bool> AtualizarStatusAgendamento(int idAgendamento, string stringStatus)
        {
            const string sql = @"
                UPDATE Agendamentos
                SET status = @Status
                WHERE id_agendamento_pai = @IdAgendamento
            ";
            return _dbConnection.ExecuteAsync(sql, new
            {
                IdAgendamento = idAgendamento,
                Status = stringStatus
            }).ContinueWith(task => task.Result > 0);
        }
    }
}
