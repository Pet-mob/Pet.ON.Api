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
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;

        public UsuarioRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<Usuario> BuscarPorTelefone(string telefone)
        {
            const string query = "SELECT Id, Telefone, CNPJ, Nome, Senha, Email FROM Usuario WHERE Telefone = @Telefone";
            using var connection = await GetOpenConnectionAsync();

            return await SqlRetryHelper.ExecuteWithRetryAsync(() =>
                connection.QueryFirstOrDefaultAsync<Usuario>(query, new { Telefone = telefone }));
        }

        public async Task<Usuario> BuscarPorCnpj(string cnpj)
        {
            const string query = "SELECT Id, Telefone, CNPJ, Nome, Senha, Email FROM Usuario WHERE CNPJ = @CNPJ";
            using var connection = await GetOpenConnectionAsync();

            return await SqlRetryHelper.ExecuteWithRetryAsync(() =>
                connection.QueryFirstOrDefaultAsync<Usuario>(query, new { CNPJ = cnpj }));
        }

        public async Task<int> InserirUsuarioAsync(Usuario usuario)
        {
            const string query = @"
                INSERT INTO Usuario (Nome, CNPJ, Telefone, Senha, Email)
                VALUES (@Nome, @CNPJ, @Telefone, @Senha, @Email);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = await GetOpenConnectionAsync();

            return await connection.ExecuteScalarAsync<int>(query, usuario);
        }

        public async Task<IEnumerable<Usuario>> GetByParameters(BuscarUsuarioReqDto dto)
        {
            var query = @"SELECT Id, Nome, CNPJ, Telefone, Senha, Email FROM Usuario WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(dto.CNPJ)) query += " AND CNPJ = @CNPJ";
            if (!string.IsNullOrWhiteSpace(dto.Telefone)) query += " AND Telefone = @Telefone";
            if (!string.IsNullOrWhiteSpace(dto.Email)) query += " AND Email = @Email";
            if (!string.IsNullOrWhiteSpace(dto.Senha)) query += " AND Senha = @Senha";
            if (dto.IdEmpresa > 0) query += " AND IdEmpresa = @IdEmpresa";
            if (dto.Id > 0) query += " AND IdUsuario = @Id";
            if (dto.IdAnimal > 0) query += " AND IdAnimal = @IdAnimal";

            using var connection = await GetOpenConnectionAsync();
            return await connection.QueryAsync<Usuario>(query, dto);
        }

        public async Task<bool> AlterarSenhaDoUsuario(string senhaNova, int idUsuario)
        {
            const string query = @"UPDATE Usuario SET Senha = @senhaNova WHERE Id = @idUsuario";

            using var connection = await GetOpenConnectionAsync();
            int linhasAfetadas = await connection.ExecuteAsync(query, new { senhaNova, idUsuario });

            return linhasAfetadas > 0;
        }

        public async Task<bool> AlterarUsuario(string nome, string telefone, int idUsuario, string email)
        {
            const string query = @"
                UPDATE Usuario 
                SET Nome = @nome, Telefone = @telefone, Email = @email
                WHERE Id = @idUsuario";

            using var connection = await GetOpenConnectionAsync();
            int linhasAfetadas = await connection.ExecuteAsync(query, new { nome, telefone, idUsuario, email });

            return linhasAfetadas > 0;
        }

        public async Task<bool> Excluir(int idUsuario)
        {
            const string deleteAnimais = "DELETE FROM Animal WHERE Id_Usuario = @idUsuario";
            const string deleteUsuario = "DELETE FROM Usuario WHERE Id = @idUsuario";

            using var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int animaisAfetados = await connection.ExecuteAsync(deleteAnimais, new { idUsuario }, transaction);
                int usuarioAfetado = await connection.ExecuteAsync(deleteUsuario, new { idUsuario }, transaction);

                transaction.Commit();

                return animaisAfetados > 0 || usuarioAfetado > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
