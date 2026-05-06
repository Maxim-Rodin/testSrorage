using System;
using System.Linq;
using System.Windows;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    public partial class EditArrivalWindow : Window
    {
        private readonly IStorageService storageService = new StorageService();
        private readonly Arrivals arrival;

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

            var products = storageService.GetProducts();
            cmbArrivalProducts.ItemsSource = products;

            Products currentProduct = products.FirstOrDefault(p => p.IdProduct == arrival.ProductId);
            if (currentProduct != null)
                cmbArrivalProducts.SelectedItem = currentProduct;
        }

        private void btnSaveArrival_Click(object sender, RoutedEventArgs e)
        {
            if (cmbArrivalProducts.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт.");
                return;
            }

            if (dpArrivalDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.");
                return;
            }

            if (!int.TryParse(txtArrivalQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            arrival.DateTime = dpArrivalDate.SelectedDate.Value.Date;
            arrival.Quantity = quantity;
            arrival.ProductId = ((Products)cmbArrivalProducts.SelectedItem).IdProduct;

            OperationResult result = storageService.UpdateArrival(arrival);
            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            DialogResult = true;
            Close();
        }

        private void btnCancelArrival_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
