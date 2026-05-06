using System.Linq;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage
{
    public partial class EditArrivalWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private readonly Arrival arrival;

        public EditArrivalWindow(Arrival selectedArrival)
        {
            InitializeComponent();
            arrival = selectedArrival;
            LoadData();
        }

        private void LoadData()
        {
            txtArrivalId.Text = arrival.Id.ToString();
            dpArrivalDate.SelectedDate = arrival.DateTime;
            txtArrivalQuantity.Text = arrival.Quantity.ToString();

            var products = storageService.GetProducts();
            cmbArrivalProducts.ItemsSource = products;

            Product currentProduct = products.FirstOrDefault(p => p.Id == arrival.ProductId);
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

            int quantity;
            if (!int.TryParse(txtArrivalQuantity.Text, out quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            arrival.DateTime = dpArrivalDate.SelectedDate.Value.Date;
            arrival.Quantity = quantity;
            arrival.ProductId = ((Product)cmbArrivalProducts.SelectedItem).Id;

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
