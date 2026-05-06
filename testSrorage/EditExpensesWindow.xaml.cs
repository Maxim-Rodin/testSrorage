using System;
using System.Linq;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage
{
    // Окно редактирования записи расхода.
    public partial class EditExpensesWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private readonly Expense expense;

        // Конструктор принимает выбранную запись для редактирования.
        public EditExpensesWindow(Expense selectedExpense)
        {
            InitializeComponent();
            expense = selectedExpense;
            LoadData();
        }

        // Загружает данные в форму редактирования.
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

        // Обработчик кнопки сохранения: обновляет документ через сервис.
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

        // Обработчик кнопки отмены редактирования.
        private void btnCancelExpense_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
