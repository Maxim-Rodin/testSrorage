using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testSrorage.инфраструктура
{
    internal class DbConnectionFactory
    {
        
            private readonly string _connectionString;

            public DbConnectionFactory(string connectionString)
            {
                _connectionString = connectionString;
            }

            public MySqlConnection Create()
            {
                return new MySqlConnection(_connectionString);
            }
        
    }
}
