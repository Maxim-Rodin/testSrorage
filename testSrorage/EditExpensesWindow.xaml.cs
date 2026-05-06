using System;
using System.Linq;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage
{
    public partial class EditExpensesWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private readonly Expense expense;

        public EditExpensesWindow(Expense selectedExpense)
        {
            InitializeComponent();
            expense = selectedExpense;
            LoadData();
        }

        private void LoadData()
        {
            txtExpenseId.Text = expense.Id.ToString();
            dpExpenseDate.SelectedDate = expense.DateTime;
            txtExpenseQuantity.Text = expense.Quantity.ToString();

            var products = storageService.GetAll<Product>();
            cmbExpenseProducts.ItemsSource = products;

            Product currentProduct = products.FirstOrDefault(p => p.Id == expense.ProductId);
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
            expense.ProductId = ((Product)cmbExpenseProducts.SelectedItem).Id;

            OperationResult result = storageService.UpdateDocument<Expense>(expense);
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
