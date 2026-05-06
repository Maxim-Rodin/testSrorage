using System;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;
using testSrorage.Infrastructure;

namespace testSrorage
{
    // Окно редактирования продукта.
    public partial class EditProductWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private readonly Product product;

        // Конструктор — принимает выбранный продукт для редактирования.
        public EditProductWindow(Product selectedProduct)
        {
            InitializeComponent();
            product = selectedProduct;
            LoadProductData();
        }

        // Загружает данные продукта в поля формы.
        private void LoadProductData()
        {
            txtProductId.Text = product.Id.ToString();
            txtProductName.Text = product.Name;
            txtProductQuantity.Text = product.Quantity.ToString();
        }

        // Обработчик кнопки сохранения — валидирует и обновляет продукт через сервис.
        private void btnSaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Введите название продукта.");
                txtProductName.Focus();
                return;
            }

            if (!int.TryParse(txtProductQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество (0 или больше).");
                txtProductQuantity.Focus();
                txtProductQuantity.SelectAll();
                return;
            }

            product.Name = txtProductName.Text.Trim();
            product.Quantity = quantity;

            OperationResult result = storageService.Update<Product>(product); // Используем Update<T>
            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            DialogResult = true;
            Close();
        }

        // Обработчик кнопки отмены — закрывает окно без сохранения.
        private void btnCancelProduct_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
