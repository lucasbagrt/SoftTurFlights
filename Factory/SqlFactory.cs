using System.Data;
using Microsoft.Data.SqlClient;

namespace SoftTurFlights.Factory
{
    public class SqlFactory
    {
        public IDbConnection SqlConnection()
        {
            string connectionString = @"Server=(local); Database=SoftTurFlights; User=sa; Password=XXX; Trusted_Connection=False; TrustServerCertificate=True";
            return new SqlConnection(connectionString);
        }
    }
}