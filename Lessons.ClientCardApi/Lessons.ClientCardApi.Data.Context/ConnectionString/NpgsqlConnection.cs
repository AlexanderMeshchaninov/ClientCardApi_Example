using Microsoft.Extensions.Configuration;

namespace Lessons.ClientCardApi.Data.Context.ConnectionString
{
    public sealed class NpgsqlConnection : Abstraction.Data.ConnectionString.IDbConnection
    {
        private IConfiguration _configuration;
        private string DbConnection { get; set; }

        public NpgsqlConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AddConnection()
        {
            return DbConnection = _configuration.GetConnectionString("PostgresSQLConnection");
        }
    }
}