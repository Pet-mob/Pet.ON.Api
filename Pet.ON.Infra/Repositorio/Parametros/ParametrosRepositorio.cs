using Dapper;
using FluentValidation.Validators;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Entidade.v1.Parametros;
using Pet.ON.Domain.Interfaces.Repositorio.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Pet.ON.Infra.Repositorio.Parametros
{
    public class ParametrosRepositorio : IParametrosRepositorio
    {
        private readonly IDbConnection _dbConnection;  // Usando conexão Dapper

        public ParametrosRepositorio(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ParametroGeral> Buscar(int idEmpresa)
        {
            var query = @"
                        SELECT 
                            id_parametro AS IdParametro,
                            id_empresa AS IdEmpresa, 
                            qtde_atendimento_simultaneo_horario AS QtdeAtendimentoSimultaneoHorario, 
                            id_modelo_servico_trabalho AS IdModeloTrabalho
                        FROM parametros
                        WHERE id_empresa = @idEmpresa";

            return await _dbConnection.QueryFirstAsync<ParametroGeral>(query, new { idEmpresa });
        }

        public async Task<bool> Atualizar(ParametroGeral parametro)
        {
            var query = @"
                    UPDATE parametros
                    SET 
                        qtde_atendimento_simultaneo_horario = @QtdeAtendimentoSimultaneoHorario,
                        id_modelo_servico_trabalho = @IdModeloTrabalho
                    WHERE id_empresa = @IdEmpresa";

            var resultado = await _dbConnection.ExecuteAsync(query, new
            {
                parametro.QtdeAtendimentoSimultaneoHorario,
                parametro.IdModeloTrabalho,
                parametro.IdParametro,
                parametro.IdEmpresa
            });
            return resultado > 0;
        }
    }
}
