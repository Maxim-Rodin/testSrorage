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
    
    public partial class AddExpensesWindow : Window
    {
        private DB connect = new DB();
        private DBManager maneger = new DBManager();
        private bool IsDataSaved = false;

        public AddExpensesWindow()
        {
            InitializeComponent();
        }

        private void aplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(nameTxBx.Text))
                {
                    MessageBox.Show("Введите имя товара!");
                    nameTxBx.Focus();
                    return;
                }

                if (!int.TryParse(quantityTxBx.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Количество должно быть больше 0!");
                    quantityTxBx.Focus();
                    return;
                }

                if (dataPiker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату!");
                    dataPiker.Focus();
                    return;
                }

                string productName = nameTxBx.Text.Trim();
                DateTime dateTime = dataPiker.SelectedDate.Value;
                int productId = -1;

                Products existingProduct = maneger.GetProductByName(productName);

                if (existingProduct != null && existingProduct.Quantity > quantity)
                {
                    productId = existingProduct.IdProduct;
                    maneger.UpdateProductQuantity(productId, quantity , false);
                    MessageBox.Show($"Обновлено количество существующего продукта '{productName}'");
                }
                else if ( existingProduct !=null &&existingProduct.Quantity <quantity) 
                {
                    MessageBox.Show($"Не возможно провести расход требуемый вычет {quantity}, на складе сейчас {existingProduct.Quantity}");
                }
                else
                {
                    MessageBox.Show("Нет такого продукта!");
                }
                Expenses expenses = new Expenses 
                { 
                    DateTime = dateTime,
                    ProductId= existingProduct?.IdProduct ?? maneger.LastInsertedId,
                    Quantiti = quantity,

                };
                if(!maneger.AddExpenses(expenses))
                {
                    throw new Exception("Ошибка при cохранении  расхода !");

                }
                MessageBox.Show("Расход успешно добавлен!");
                IsDataSaved = true;
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления расхода {ex.Message}");

            }


        }

        private void canсelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataSaved)
            {
                MessageBoxResult result = MessageBox.Show("Вы хотите отменить операцию? Все изменения будут потеряны.",
                "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
    }
}
