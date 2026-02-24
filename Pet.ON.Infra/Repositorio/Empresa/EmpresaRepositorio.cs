using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Pet.ON.Domain.Dtos.v1.Empresa;
using System.Linq;
using System;

namespace Pet.ON.Infra.Repositorio
{
    public class EmpresaRepositorio : IEmpresaRepositorio
    {
        private readonly IDbConnection _dbConnection;  // Usando conexão Dapper

        public EmpresaRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #region GetByParameters
        public async Task<IEnumerable<Empresa>> GetByParameters(BuscarEmpresaReqDto dto)
        {
            var query = @"
                    SELECT 
                        id_empresa AS IdEmpresa,
                        fantasia_razao_social AS DescricaoNomeFisica,
                        CNPJ AS CNPJ,
                        Telefone,
                        Email,
                        id_categoria AS IdCategoria, 
                        endereco AS Endereco
                    FROM Empresa
                    WHERE 1=1";

            var parameters = new DynamicParameters();

            if (dto.IdEmpresa > 0)
            {
                query += " AND id_empresa = @IdEmpresa";
                parameters.Add("IdEmpresa", dto.IdEmpresa);
            }

            if (dto.IdCategoria > 0)
            {
                query += " AND id_categoria = @IdCategoria";
                parameters.Add("IdCategoria", dto.IdCategoria);
            }

            if (!string.IsNullOrWhiteSpace(dto.DescricaoNomeFantasia))
            {
                query += " AND fantasia_razao_social LIKE @DescricaoNomeFantasia";
                parameters.Add("DescricaoNomeFantasia", $"%{dto.DescricaoNomeFantasia}%");
            }

            if (!string.IsNullOrWhiteSpace(dto.Cnpj))
            {
                query += " AND Cnpj = @Cnpj";
                parameters.Add("Cnpj", $"{dto.Cnpj}");
            }

            return await _dbConnection.QueryAsync<Empresa>(query, parameters);
        }
        #endregion

        #region UpdateAsync
        public async Task<Empresa> UpdateAsync(Empresa empresa)
        {
            var query = @"
                UPDATE Empresa
                SET fantasia_razao_social = @DescricaoNomeFisica,
                    CNPJ = @CNPJ,
                    Email = @Email,
                    Telefone = @Telefone
                WHERE id_empresa = @IdEmpresa";

            var parameters = new
            {
                empresa.DescricaoNomeFisica,
                empresa.CNPJ,
                empresa.Email,
                empresa.Telefone,
                empresa.IdEmpresa
            };

            await _dbConnection.ExecuteAsync(query, parameters);
            return empresa;
        }
        #endregion

        #region InsertAsync
        public async Task<Empresa> InsertAsync(Empresa empresa)
        {
            var query = @"
                INSERT INTO Empresa (fantasia_razao_social, CNPJ, Email, Telefone)
                VALUES (@DescricaoNomeFisica, @CNPJ, @Email, @Telefone);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            var parameters = new
            {
                empresa.DescricaoNomeFisica,
                empresa.CNPJ,
                empresa.Email,
                empresa.Telefone
            };

            var id = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            empresa.IdEmpresa = id;
            return empresa;
        }

        public async Task<IEnumerable<HorariosFuncionamento>> BuscarHorariosFuncionamento(BuscarHorariosFuncionamentosEmpresaReqDto dto)
        {
            var query = @"
                    SELECT 
                        id_empresa AS IdEmpresa,
                        nome_dia_semana AS NomeDiaSemana,
                        funciona_nesse_dia AS FuncionaNesseDia,
                        horario_abertura AS HorarioAbertura,
                        horario_fechamento AS HorarioFechamento
                    FROM horarios_funcionamento
                    WHERE 1=1";

            var parameters = new DynamicParameters();

            if (dto.IdEmpresa > 0)
            {
                query += " AND id_empresa = @IdEmpresa";
                parameters.Add("IdEmpresa", dto.IdEmpresa);
            }

            if (!string.IsNullOrWhiteSpace(dto.NomeDiaSemana))
            {
                query += " AND nome_dia_semana LIKE @NomeDiaSemana";
                parameters.Add("NomeDiaSemana", $"%{dto.NomeDiaSemana}%");
            }

            return await _dbConnection.QueryAsync<HorariosFuncionamento>(query, parameters);
        }
        #endregion

        public async Task AtualizarHorariosFuncionamentoAsync(List<HorariosFuncionamento> horarios)
        {
            if (horarios == null || !horarios.Any())
                return;

            var idEmpresa = horarios.First().IdEmpresa;

            var deleteQuery = @"DELETE FROM horarios_funcionamento WHERE id_empresa = @IdEmpresa";
            var insertQuery = @"INSERT INTO horarios_funcionamento (
                                    id_empresa,
                                    nome_dia_semana,
                                    funciona_nesse_dia,
                                    horario_abertura,
                                    horario_fechamento                                    
                                ) VALUES (
                                    @IdEmpresa,
                                    @NomeDiaSemana,
                                    @FuncionaNesseDia,
                                    @HorarioAbertura,
                                    @HorarioFechamento
                                )";

            using (var connection = _dbConnection)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(deleteQuery, new { IdEmpresa = idEmpresa }, transaction);

                        foreach (var horario in horarios)
                        {
                            await connection.ExecuteAsync(insertQuery, horario, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }

        public async Task<IEnumerable<Empresa>> BuscarEmpresasVinculadoAoUsuario(int idUsuario)
        {
            var query = @"
                        select 
	                        a.id_empresa AS IdEmpresa,
	                        e.fantasia_razao_social AS DescricaoNomeFisica,
	                        e.CNPJ AS CNPJ,
	                        e.Telefone,
	                        e.Email,
                            e.id_categoria AS IdCategoria,
                            e.endereco AS Endereco
                        from agendamentos a
                        inner join empresa e ON a.id_empresa = e.id_empresa
                        where a.id_usuario = @idUsuario
                        group by a.id_empresa,
	                        e.fantasia_razao_social,
	                        e.CNPJ,
	                        e.Telefone,
	                        e.Email,
                            e.id_categoria,
                            e.endereco ";

            return await _dbConnection.QueryAsync<Empresa>(query, new { idUsuario });

        }
    }
}
