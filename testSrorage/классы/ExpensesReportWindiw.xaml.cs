using System;
using System.Collections.Generic;
using System.Windows;
using testSrorage.классы.интерфейсы;

namespace testSrorage.классы
{
    public partial class ExpensesReportWindiw : Window
    {
        private readonly IStorageService storageService = new StorageService();
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
                    MessageBox.Show("Выберите обе даты.");
                    return;
                }

                DateTime startDate = dpStartDate.SelectedDate.Value.Date;
                DateTime endDate = dpEndDate.SelectedDate.Value.Date;

                if (startDate > endDate)
                {
                    MessageBox.Show("Дата 'С' не может быть позже даты 'По'.");
                    return;
                }

                currentReport = storageService.GetExpensesByDateRange(startDate, endDate);

                if (currentReport.Count == 0)
                {
                    MessageBox.Show("Нет данных за выбранный период.");
                    dgReport.ItemsSource = null;
                    UpdateSummary(new ReportSummary(0, 0));
                    return;
                }

                dgReport.ItemsSource = currentReport;
                UpdateSummary(storageService.CreateSummary(currentReport));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void UpdateSummary(ReportSummary summary)
        {
            string startDateStr = dpStartDate.SelectedDate.HasValue
                ? dpStartDate.SelectedDate.Value.ToString("dd.MM.yyyy")
                : string.Empty;
            string endDateStr = dpEndDate.SelectedDate.HasValue
                ? dpEndDate.SelectedDate.Value.ToString("dd.MM.yyyy")
                : string.Empty;

            txtSummary.Text = "Период: " + startDateStr + " - " + endDateStr;
            txtTotalQuantity.Text = "Общее количество: " + summary.TotalQuantity;
            txtTotalCount.Text = "Записей: " + summary.RecordCount;
        }
    }
}
