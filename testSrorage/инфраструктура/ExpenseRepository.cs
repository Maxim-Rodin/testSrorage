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
    internal class ExpenseRepository : IExpenseRepository
    {
        private readonly DbConnectionFactory _factory;

        public ExpenseRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public List<Expense> GetAll()
        {
            List<Expense> list = new List<Expense>();

            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM expenses", conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Expense e = new Expense(
                    reader.GetInt32("productId"),
                    reader.GetInt32("quantity"));

                e.SetId(reader.GetInt32("idExpenses"));
                e.SetDate(reader.GetDateTime("dateExpenses"));

                list.Add(e);
            }

            reader.Close();
            conn.Close();

            return list;
        }

        public void Add(Expense expense)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO expenses (dateExpenses, productId, quantity) VALUES (@d,@p,@q)",
                conn);

            cmd.Parameters.AddWithValue("@d", expense.Date);
            cmd.Parameters.AddWithValue("@p", expense.ProductId);
            cmd.Parameters.AddWithValue("@q", expense.Quantity);

            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void Delete(int id)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "DELETE FROM expenses WHERE idExpenses=@id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void DeleteByProductId(int productId)
        {
            MySqlConnection conn = _factory.Create();
            conn.Open();

            MySqlCommand cmd = new MySqlCommand(
                "DELETE FROM expenses WHERE productId=@id", conn);

            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
