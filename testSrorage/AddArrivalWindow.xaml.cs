using System;
using System.Windows;
using System.Windows.Controls;
using testSrorage.классы;
 

namespace testSrorage
{
   
    public partial class AddArrivalWindow : Window
    {
        private DB connect = new DB();
        private DBManager maneger = new DBManager();
        private bool IsDataSaved = false;

        public AddArrivalWindow()
        {
            
            InitializeComponent();
        }

        private void canсelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataSaved) 
            {
                MessageBoxResult result  = MessageBox.Show("Вы хотите отменить операцию? Все изменения будут потеряны.",
                "Подтверждение отмены",MessageBoxButton.YesNo,MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close() ;
            }
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
                DateTime arriveDate = dataPiker.SelectedDate.Value;
                int productId = -1;


                Products existingProduct = maneger.GetProductByName(productName);

                if (existingProduct != null)
                {
                    productId = existingProduct.IdProduct;

                    // Обновляем количество существующего продукта
                    maneger.UpdateProductQuantity(productId, quantity, true);
                    MessageBox.Show($"Обновлено количество существующего продукта '{productName}'");

                    
                }
                else
                {

                    if (maneger.AddNewProductWithQuantity(productName, quantity))
                    {
                        productId = maneger.LastInsertedId;

                        if (productId <= 0)
                        {
                           
                            productId = maneger.GetMaxProductId();
                            MessageBox.Show($"Создан новый продукт '{productName}' (ID вычислен: {productId})");
                        }
                        else
                        {
                            MessageBox.Show($"Создан новый продукт '{productName}' с ID: {productId}");
                        }
                    }
                    else
                    {
                        throw new Exception("Не удалось создать новый продукт");
                    }

                }

                
                Arrivals arrival = new Arrivals
                {
                    DateTime = arriveDate,
                    ProductId = existingProduct?.IdProduct ?? maneger.LastInsertedId,
                    Quantity = quantity
                };

                if (!maneger.AddArrivals(arrival))
                {
                    throw new Exception("Ошибка при сохранении прихода");
                }

                MessageBox.Show("Приход успешно добавлен!");
                IsDataSaved = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
