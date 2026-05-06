using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSrorage.Application;
using testSrorage.классы;

namespace testSrorage.инфраструктура
{
    internal class ArrivalRepository : IArrivalRepository
    {
        
            private readonly DbConnectionFactory _factory;

            public ArrivalRepository(DbConnectionFactory factory)
            {
                _factory = factory;
            }

            public List<Arrival> GetAll()
            {
                List<Arrival> list = new List<Arrival>();

                MySqlConnection conn = _factory.Create();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM arrivals", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Arrival a = new Arrival(
                        reader.GetInt32("productId"),
                        reader.GetInt32("quantity"));

                    a.SetId(reader.GetInt32("idArrivals"));
                    a.SetDate(reader.GetDateTime("date"));

                    list.Add(a);
                }

                reader.Close();
                conn.Close();

                return list;
            }

            public void Add(Arrival arrival)
            {
                MySqlConnection conn = _factory.Create();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO arrivals (date, productId, quantity) VALUES (@d,@p,@q)",
                    conn);

                cmd.Parameters.AddWithValue("@d", arrival.Date);
                cmd.Parameters.AddWithValue("@p", arrival.ProductId);
                cmd.Parameters.AddWithValue("@q", arrival.Quantity);

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            public void Delete(int id)
            {
                MySqlConnection conn = _factory.Create();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM arrivals WHERE idArrivals=@id", conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            public void DeleteByProductId(int productId)
            {
                MySqlConnection conn = _factory.Create();
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM arrivals WHERE productId=@id", conn);

                cmd.Parameters.AddWithValue("@id", productId);

                cmd.ExecuteNonQuery();

                conn.Close();
            }

        public void Update(Arrival arrival)
        {
            MySqlConnection conn = _factory.Create();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "UPDATE arrivals SET date=@d, productId=@p, quantity=@q WHERE idArrivals=@id",
                    conn);

                cmd.Parameters.AddWithValue("@d", arrival.Date);
                cmd.Parameters.AddWithValue("@p", arrival.ProductId);
                cmd.Parameters.AddWithValue("@q", arrival.Quantity);
                cmd.Parameters.AddWithValue("@id", arrival.Id);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
