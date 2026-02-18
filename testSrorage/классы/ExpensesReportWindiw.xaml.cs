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

namespace testSrorage.классы
{
    
    public partial class ExpensesReportWindiw : Window
    {
        private DBManager manager = new DBManager();
        private List<Expenses> currentReport = new List<Expenses>();
        public ExpensesReportWindiw()
        {
            InitializeComponent();

            dpEndDate.SelectedDate = DateTime.Today;
            dpStartDate.SelectedDate = DateTime.Today.AddDays(-30);
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
                {
                    MessageBox.Show("Выберите обе даты");
                    return;
                }

                DateTime startDate = dpStartDate.SelectedDate.Value;
                DateTime endDate = dpEndDate.SelectedDate.Value;

                if (startDate > endDate)
                {
                    MessageBox.Show("Дата 'С' не может быть позже даты 'По'");
                    return;
                }


                currentReport = manager.GetExpensesByDateRange(startDate, endDate);

                if (currentReport.Count == 0)
                {
                    MessageBox.Show("Нет данных за выбранный период");
                    dgReport.ItemsSource = null;
                    UpdateSummary(0, 0);
                    return;
                }


                dgReport.ItemsSource = currentReport;


                int totalQuantity = currentReport.Sum(c => c.Quantiti);
                int recordCount = currentReport.Count;

                UpdateSummary(totalQuantity, recordCount);
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            
        }

        private void UpdateSummary(int totalQuantity, int recordCount)
        {
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            txtSummary.Text = $"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            txtTotalQuantity.Text = $"Общее количество: {totalQuantity}";
            txtTotalCount.Text = $"Записей: {recordCount}";
        }

    }
}

