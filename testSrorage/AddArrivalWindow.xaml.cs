using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;

namespace testSrorage
{
    public partial class AddArrivalWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private bool isDataSaved;

        public AddArrivalWindow()
        {
            InitializeComponent();
        }

        private void canсelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!isDataSaved)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы хотите отменить операцию? Все изменения будут потеряны.",
                    "Подтверждение отмены",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    Close();

                return;
            }

            Close();
        }

        private void aplyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTxBx.Text))
            {
                MessageBox.Show("Введите имя товара!");
                nameTxBx.Focus();
                return;
            }

            int quantity;
            if (!int.TryParse(quantityTxBx.Text, out quantity) || quantity <= 0)
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

            OperationResult result = storageService.AddArrival(
                nameTxBx.Text,
                quantity,
                dataPiker.SelectedDate.Value);

            MessageBox.Show(result.Message);

            if (!result.Success)
                return;

            isDataSaved = true;
            DialogResult = true;
            Close();
        }
    }
}
