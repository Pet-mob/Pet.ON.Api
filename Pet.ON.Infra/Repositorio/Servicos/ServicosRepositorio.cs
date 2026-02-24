using Dapper;
using Microsoft.Data.SqlClient;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Pet.ON.Infra.Repositorio
{
    public class ServicosRepositorio : IServicosRepositorio
    {
        private readonly IDbConnection _dbConnection;

        public ServicosRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task Adicionar(AdicionarServicoReqDto dto)
        {
            var query = @"
                    INSERT INTO Servicos 
                    (
                        nome_servico,
                        preco,
                        duracao_servico,
                        observacao,
                        possui_mensal,
                        preco_mensal,
                        id_empresa,
                        id_porte
                    )
                    VALUES 
                    (
                        @Descricao,
                        @Valor,
                        @Duracao,
                        @Observacao,
                        @PossuiMensal,
                        @PrecoMensal,
                        @IdEmpresa,
                        @IdPorte
                    )";

            await _dbConnection.ExecuteAsync(query, dto);
        }

        public async Task Atualizar(AtualizarServicoReqDto dto)
        {
            var query = @"
                    UPDATE Servicos SET
                        nome_servico = @Descricao,
                        preco = @Valor,
                        duracao_servico = @Duracao,
                        observacao = @Observacao,
                        possui_mensal = @PossuiMensal,
                        preco_mensal = @PrecoMensal,
                        id_porte = @IdPorte
                    WHERE 
                        id_servico = @IdServico AND id_empresa = @IdEmpresa";

            await _dbConnection.ExecuteAsync(query, dto);
        }
        public async Task<IEnumerable<Servicos>> GetByParameters(BuscarServicosReqDto dto)
        {

            var query = @"
                    SELECT 
                        id_servico AS IdServico,
                        nome_servico AS Descricao,
                        preco AS Valor,
                        duracao_servico AS Duracao,
                        observacao AS Observacao, 
                        possui_mensal AS PossuiMensal, 
                        preco_mensal AS PrecoMensal,
                        id_empresa AS IdEmpresa,
                        id_porte AS IdPorte
                    FROM Servicos
                    WHERE 1=1";

            if (dto.IdEmpresa > 0)
                query += " AND id_empresa = @IdEmpresa";

            // Ajuste: retorna serviços do porte informado OU para todos os portes (id_porte = 0)
            if (dto.IdPorte > 0)
                query += " AND (id_porte = 0 OR id_porte = @IdPorte)";

            return await _dbConnection.QueryAsync<Servicos>(query, dto);
        }

        public async Task<bool> Excluir(int idEmpresa, int idServico)
        {
            const string query = @"
                    DELETE FROM Servicos
                    WHERE id_empresa = @idEmpresa
                      AND id_servico = @idServico ";

            var parametros = new
            {
                idEmpresa,
                idServico
            };


            var execucao = await _dbConnection.ExecuteAsync(query, parametros);
            return execucao > 0;
        }

        public async Task<bool> PossoExcluirServico(int idEmpresa, int idServico)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM Agendamentos
                WHERE id_empresa = @IdEmpresa
                    AND id_servico = @IdServico;
            ";

            var parametros = new
            {
                IdEmpresa = idEmpresa,
                IdServico = idServico
            };

            // QuerySingleAsync<int> é mais apropriado, pois COUNT sempre retorna um valor
            var quantidade = await _dbConnection.QuerySingleAsync<int>(sql, parametros);

            // Só pode excluir se NÃO existir nenhum agendamento com esse serviço
            return quantidade == 0;
        }
    }
}
