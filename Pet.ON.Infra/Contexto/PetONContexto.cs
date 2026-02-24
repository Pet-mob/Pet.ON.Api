using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Pet.ON.Infra.Contexto
{
    public class PetONContexto
    {
        private readonly IDbConnection _dbConnection;

        public PetONContexto(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration["DefaultConnection"]);
        }

        public IDbConnection DbConnection => _dbConnection;
    }
}
