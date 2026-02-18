using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using testSrorage.классы;

namespace testSrorage
{
   
    public partial class EditExpensesWindow : Window
    {
        private Expenses expense;
        private DBManager manager = new DBManager();

        public EditExpensesWindow(Expenses selectedExpense)
        {
            InitializeComponent();
            expense = selectedExpense;
            LoadData();
        }

        private void LoadData()
        {
            txtExpenseId.Text = expense.IdExpenses.ToString();
            dpExpenseDate.SelectedDate = expense.DateTime;
            txtExpenseQuantity.Text =expense.Quantiti.ToString();

           
            var products = manager.GetAllProducts();
            cmbExpenseProducts.ItemsSource = products;

           
            var currentProduct = products.FirstOrDefault(p => p.IdProduct == expense.ProductId);
            if (currentProduct != null)
                cmbExpenseProducts.SelectedItem = currentProduct;
        }

        private void btnSaveExpense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbExpenseProducts.SelectedItem == null)
                {
                    MessageBox.Show("Выберите продукт");
                    return;
                }

                if (dpExpenseDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату");
                    return;
                }

                if (!int.TryParse(txtExpenseQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество");
                    return;
                }

               
                expense.DateTime = dpExpenseDate.SelectedDate.Value;
                expense.Quantiti = quantity;
                expense.IdExpenses = ((Products)cmbExpenseProducts.SelectedItem).IdProduct;

                if (manager.UpdateExpense(expense))
                {
                    MessageBox.Show("Расход обновлен");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnCancelExpense_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

