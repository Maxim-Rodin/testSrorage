using System.Windows;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    public partial class EditProductWindow : Window
    {
        private readonly IStorageService storageService = new StorageService();
        private readonly Products product;

        public EditProductWindow(Products selectedProduct)
        {
            InitializeComponent();
            product = selectedProduct;
            LoadProductData();
        }

        private void LoadProductData()
        {
            txtProductId.Text = product.IdProduct.ToString();
            txtProductName.Text = product.Name;
            txtProductQuantity.Text = product.Quantity.ToString();
        }

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

            OperationResult result = storageService.UpdateProduct(product);
            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            DialogResult = true;
            Close();
        }

        private void btnCancelProduct_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
