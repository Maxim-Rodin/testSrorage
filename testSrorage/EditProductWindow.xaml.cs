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

namespace testSrorage
{
    
    public partial class EditProductWindow : Window
    {
        DBManager manager = new DBManager();
        private Products product = new Products();
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
            
            try
            {
                
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Введите название продукта");
                    txtProductName.Focus();
                    return;
                }

                if (!int.TryParse(txtProductQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Введите корректное количество (0 или больше)");
                    txtProductQuantity.Focus();
                    txtProductQuantity.SelectAll();
                    return;
                }

               
                product.Name = txtProductName.Text.Trim();
                product.Quantity = quantity;

                
                if (manager.UpdateProduct(product))
                {
                    MessageBox.Show("Продукт успешно обновлен!");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить продукт");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void btnCancelProduct_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        
    }
}
