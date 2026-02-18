using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using testSrorage.классы;

namespace testSrorage
{
    public partial class IncomeReportWindow : Window
    {
        private DBManager manager = new DBManager();
        private List<Arrivals> currentReport = new List<Arrivals>();

        public IncomeReportWindow()
        {
            InitializeComponent();
            dpEndDate.SelectedDate = DateTime.Today;
            dpStartDate.SelectedDate = DateTime.Today.AddDays(-30);

           
            dpStartDate.SelectedDateFormat = DatePickerFormat.Long;
            dpEndDate.SelectedDateFormat = DatePickerFormat.Long;
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

               
                DateTime startDate = dpStartDate.SelectedDate.Value.Date; 
                DateTime endDate = dpEndDate.SelectedDate.Value.Date;     

                if (startDate > endDate)
                {
                    MessageBox.Show("Дата 'С' не может быть позже даты 'По'");
                    return;
                }

               
                currentReport = manager.GetArrivalsByDateRange(startDate, endDate);

                if (currentReport.Count == 0)
                {
                    MessageBox.Show("Нет данных за выбранный период");
                    dgReport.ItemsSource = null;
                    UpdateSummary(0, 0);
                    return;
                }

                
                dgReport.ItemsSource = currentReport;

                
                int totalQuantity = currentReport.Sum(a => a.Quantiti);
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

            
            string startDateStr = startDate?.ToString("dd.MM.yyyy") ?? "";
            string endDateStr = endDate?.ToString("dd.MM.yyyy") ?? "";

            txtSummary.Text = $"Период: {startDateStr} - {endDateStr}";
            txtTotalQuantity.Text = $"Общее количество: {totalQuantity}";
            txtTotalCount.Text = $"Записей: {recordCount}";
        }
    }
}