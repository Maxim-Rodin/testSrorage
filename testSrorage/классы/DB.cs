using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace testSrorage
{
    internal class DB
    {
        private MySqlConnection connection =
            new MySqlConnection("server=localhost;port=3306;username=root;password=Xameleon8805;database=storage_db");

        public bool IsConnected
        {
            get { return connection.State == ConnectionState.Open; }
        }

        public void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                try
                {
                    connection.Open();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049)
                    {
                        CreateDatabase();
                        connection.Open();
                    }
                    else
                    {
                        throw new Exception("DB connection error: " + ex.Message);
                    }
                }
            }
        }

        public void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            if (connection.State != ConnectionState.Open)
                OpenConnection();

            return connection;
        }

        private void CreateDatabase()
        {
            MySqlConnection temp = null;
            MySqlCommand cmd = null;

            try
            {
                temp = new MySqlConnection("server=localhost;port=3306;username=root;password=Xameleon8805");
                temp.Open();

                cmd = temp.CreateCommand();

                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS storage_db";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "USE storage_db";
                cmd.ExecuteNonQuery();

                CreateTables(cmd);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();

                if (temp != null)
                {
                    temp.Close();
                    temp.Dispose();
                }
            }
        }

        private void CreateTables(MySqlCommand cmd)
        {
            cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS products (
                        idProducts INT PRIMARY KEY AUTO_INCREMENT,
                        nameProduct VARCHAR(100),
                        quantity INT DEFAULT 0
                    )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS arrivals (
                    idArrivals INT PRIMARY KEY AUTO_INCREMENT,
                    date DATETIME,
                    productId INT,
                    quantity INT,
                    FOREIGN KEY (productId) REFERENCES products(idProducts)
                )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS expenses (
                idExpenses INT PRIMARY KEY AUTO_INCREMENT,
                dateExpenses DATETIME,
                productId INT,
                quantity INT,
                FOREIGN KEY (productId) REFERENCES products(idProducts)
            )";
                        cmd.ExecuteNonQuery();
        }
    }
}