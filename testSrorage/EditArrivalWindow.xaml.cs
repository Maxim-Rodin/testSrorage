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
    /// <summary>
    /// Логика взаимодействия для EditArrivalWindow.xaml
    /// </summary>
    public partial class EditArrivalWindow : Window
    {
        private Arrivals arrival;
        private DBManager manager = new DBManager();

        public EditArrivalWindow(Arrivals selectedArrival)
        {
            InitializeComponent();
            arrival = selectedArrival;
            LoadData();
        }

        private void LoadData()
        {
            txtArrivalId.Text = arrival.IdArrivals.ToString();
            dpArrivalDate.SelectedDate = arrival.DateTime;
            txtArrivalQuantity.Text = arrival.Quantity.ToString();

           
            var products = manager.GetAllProducts();
            cmbArrivalProducts.ItemsSource = products;

            
            var currentProduct = products.FirstOrDefault(p => p.IdProduct == arrival.ProductId);
            if (currentProduct != null)
                cmbArrivalProducts.SelectedItem = currentProduct;
        }

        private void btnSaveArrival_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbArrivalProducts.SelectedItem == null)
                {
                    MessageBox.Show("Выберите продукт");
                    return;
                }

                if (dpArrivalDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату");
                    return;
                }

                if (!int.TryParse(txtArrivalQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество");
                    return;
                }

                
                arrival.DateTime = dpArrivalDate.SelectedDate.Value;
                arrival.Quantity = quantity;
                arrival.ProductId = ((Products)cmbArrivalProducts.SelectedItem).IdProduct;

                if (manager.UpdateArrival(arrival))
                {
                    MessageBox.Show("Приход обновлен");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnCancelArrival_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

