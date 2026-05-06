using System;
using System.Linq;
using System.Windows;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    public partial class EditExpensesWindow : Window
    {
        private readonly IStorageService storageService = new StorageService();
        private readonly Expenses expense;

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
            txtExpenseQuantity.Text = expense.Quantity.ToString();

            var products = storageService.GetProducts();
            cmbExpenseProducts.ItemsSource = products;

            Products currentProduct = products.FirstOrDefault(p => p.IdProduct == expense.ProductId);
            if (currentProduct != null)
                cmbExpenseProducts.SelectedItem = currentProduct;
        }

        private void btnSaveExpense_Click(object sender, RoutedEventArgs e)
        {
            if (cmbExpenseProducts.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт.");
                return;
            }

            if (dpExpenseDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.");
                return;
            }

            if (!int.TryParse(txtExpenseQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            expense.DateTime = dpExpenseDate.SelectedDate.Value.Date;
            expense.Quantity = quantity;
            expense.ProductId = ((Products)cmbExpenseProducts.SelectedItem).IdProduct;

            OperationResult result = storageService.UpdateExpense(expense);
            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            DialogResult = true;
            Close();
        }

        private void btnCancelExpense_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
