using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pet.ON.Domain.Dtos.v1;
using Pet.ON.Domain.Entidade.v1;
using Pet.ON.Domain.Interfaces.Repositorio;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Pet.ON.Infra.Repositorio
{
    public class AnimalRepositorio : IAnimalRepositorio
    {
        private readonly string _connectionString;

        public AnimalRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<bool> Adicionar(Animal animal)
        {
            const string sql = @"INSERT INTO animal (nome, idade, raca, observacoes, id_usuario, id_porte)
                                 VALUES (@Nome, @Idade, @Raca, @Observacoes, @IdUsuario, @IdPorte);";

            using var connection = await GetOpenConnectionAsync();

            var parametros = new
            {
                animal.Nome,
                animal.Idade,
                animal.Raca,
                animal.Observacoes,
                animal.IdUsuario,
                animal.IdPorte
            };

            var result = await connection.ExecuteAsync(sql, parametros);
            return result > 0;
        }

        public async Task<int> AdicionarAnimalNovo(Animal animal)
        {
            const string sql = @"INSERT INTO animal (nome, idade, raca, observacoes, id_usuario, id_porte)
                                 VALUES (@Nome, @Idade, @Raca, @Observacoes, @IdUsuario, @IdPorte);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = await GetOpenConnectionAsync();

            var parametros = new
            {
                animal.Nome,
                animal.Idade,
                animal.Raca,
                animal.Observacoes,
                animal.IdUsuario,
                animal.IdPorte
            };

            return await connection.ExecuteScalarAsync<int>(sql, parametros);
        }

        public async Task<bool> AlterarAnimal(string nome, int idade, string raca, string observacoes, int idAnimal, int idUsuario, int idPorte)
        {
            const string sql = @"UPDATE animal 
                                 SET nome = @Nome, idade = @Idade, raca = @Raca, observacoes = @Observacoes, id_porte = @IdPorte
                                 WHERE id_usuario = @IdUsuario AND id_animal = @IdAnimal";

            using var connection = await GetOpenConnectionAsync();

            var parametros = new
            {
                Nome = nome,
                Idade = idade,
                Raca = raca,
                Observacoes = observacoes,
                IdAnimal = idAnimal,
                IdUsuario = idUsuario,
                IdPorte = idPorte
            };

            var result = await connection.ExecuteAsync(sql, parametros);
            return result > 0;
        }

        public async Task<bool> Excluir(int idUsuario, int idAnimal)
        {
            const string sql = "DELETE FROM animal WHERE id_animal = @IdAnimal AND id_usuario = @IdUsuario";

            using var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = await connection.ExecuteAsync(sql, new { IdAnimal = idAnimal, IdUsuario = idUsuario }, transaction);
                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Animal>> GetByParameters(BuscarAnimalReqDto dto)
        {
            using var connection = await GetOpenConnectionAsync();

            var query = @"SELECT id_animal AS IdAnimal, nome AS Nome, idade AS Idade, raca AS Raca, observacoes AS Observacoes, id_usuario AS IdUsuario, id_porte AS IdPorte
                          FROM animal WHERE 1=1";

            var parameters = new DynamicParameters();

            if (dto.IdUsuario > 0)
            {
                query += " AND id_usuario = @IdUsuario";
                parameters.Add("IdUsuario", dto.IdUsuario);
            }

            if (dto.IdAnimal > 0)
            {
                query += " AND id_animal = @IdAnimal";
                parameters.Add("IdAnimal", dto.IdAnimal);
            }

            if (!string.IsNullOrWhiteSpace(dto.Nome))
            {
                query += " AND nome = @Nome";
                parameters.Add("Nome", dto.Nome);
            }

            if (!string.IsNullOrWhiteSpace(dto.Raca))
            {
                query += " AND raca = @Raca";
                parameters.Add("Raca", dto.Raca);
            }

            return await connection.QueryAsync<Animal>(query, parameters);
        }
    }
}
