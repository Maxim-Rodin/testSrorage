using System;
using System.Linq;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage
{
    // Окно редактирования записи прихода.
    public partial class EditArrivalWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private readonly Arrival arrival;

        // Конструктор — принимает выбранную запись прихода.
        public EditArrivalWindow(Arrival selectedArrival)
        {
            InitializeComponent();
            arrival = selectedArrival;
            LoadData();
        }

        // Загружает данные в поля формы.
        private void LoadData()
        {
            txtArrivalId.Text = arrival.Id.ToString();
            dpArrivalDate.SelectedDate = arrival.DateTime;
            txtArrivalQuantity.Text = arrival.Quantity.ToString();

            var products = storageService.GetAll<Product>();
            cmbArrivalProducts.ItemsSource = products;

            Product currentProduct = products.FirstOrDefault(p => p.Id == arrival.ProductId);
            if (currentProduct != null)
                cmbArrivalProducts.SelectedItem = currentProduct;
        }

        // Обработчик кнопки сохранения — обновление документа прихода.
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
            arrival.ProductId = ((Product)cmbArrivalProducts.SelectedItem).Id;

            OperationResult result = storageService.UpdateDocument<Arrival>(arrival);
            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            DialogResult = true;
            Close();
        }

        // Обработчик кнопки отмены редактирования.
        private void btnCancelArrival_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
