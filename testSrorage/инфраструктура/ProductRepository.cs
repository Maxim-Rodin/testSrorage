using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testSrorage.Application;

namespace testSrorage.инфраструктура
{
    internal class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _factory;

        public ProductRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Product> GetAll()
        {
            List<Product> list = new List<Product>();

            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM products", conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Product p = new Product(
                    reader.GetString("nameProduct"),
                    reader.GetInt32("quantity"));

                p.SetId(reader.GetInt32("idProducts"));

                list.Add(p);
            }

            reader.Close();
            conn.Close();

            return list;
        }

        public Product GetById(int id)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "SELECT * FROM products WHERE idProducts=@id", conn);

            cmd.Parameters.AddWithValue("@id", id);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Product p = new Product(
                    reader.GetString("nameProduct"),
                    reader.GetInt32("quantity"));

                p.SetId(reader.GetInt32("idProducts"));

                reader.Close();
                conn.Close();

                return p;
            }

            reader.Close();
            conn.Close();

            throw new Exception("Продукт не найден");
        }

        public void Add(Product product)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO products (nameProduct, quantity) VALUES (@n,@q)",
                conn);

            cmd.Parameters.AddWithValue("@n", product.Name);
            cmd.Parameters.AddWithValue("@q", product.Quantity);

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Update(Product product)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "UPDATE products SET nameProduct=@n, quantity=@q WHERE idProducts=@id",
                conn);

            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@n", product.Name);
            cmd.Parameters.AddWithValue("@q", product.Quantity);

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Delete(int id)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "DELETE FROM products WHERE idProducts=@id", conn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            conn.Close();
        }
        public Product FindByName(string name)
        {
            MySqlConnection conn = _factory.Create();
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT * FROM products WHERE nameProduct=@n LIMIT 1",
                    conn);

                cmd.Parameters.AddWithValue("@n", name);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Product p = new Product(
                        reader.GetString("nameProduct"),
                        reader.GetInt32("quantity"));

                    p.SetId(reader.GetInt32("idProducts"));

                    return p;
                }

                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
