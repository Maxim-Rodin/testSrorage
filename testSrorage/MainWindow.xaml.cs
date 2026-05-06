using System;
using System.Collections.Generic;
using System.Windows;
using testSrorage.классы;
using testSrorage.классы.интерфейсы;

namespace testSrorage
{
    public partial class MainWindow : Window
    {
        private readonly IStorageService storageService = new StorageService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowResult(storageService.CheckConnection());
        }

        private void checkBtn1_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void arrivalBtn_Click(object sender, RoutedEventArgs e)
        {
            AddArrivalWindow addArrivalWindow = new AddArrivalWindow();
            addArrivalWindow.ShowDialog();
            LoadProducts();
        }

        private void checkArriveBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadArrivals();
        }

        private void checkExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadExpenses();
        }

        private void addExpenesBtn_Click(object sender, RoutedEventArgs e)
        {
            AddExpensesWindow addExpensesWindow = new AddExpensesWindow();
            addExpensesWindow.ShowDialog();
            LoadProducts();
        }

        private void deletBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (datagrid.SelectedItem == null)
                {
                    MessageBox.Show("Выберите объект для удаления.");
                    return;
                }

                if (datagrid.SelectedItem is Products product)
                {
                    int relatedCount = storageService.CountProductDocuments(product.IdProduct);
                    if (relatedCount > 0)
                    {
                        MessageBoxResult confirmation = MessageBox.Show(
                            "У продукта есть " + relatedCount + " связанных записей (приходы/расходы).\nУдалить продукт и все связанные записи?",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (confirmation != MessageBoxResult.Yes)
                            return;
                    }

                    ShowResult(storageService.DeleteProduct(product));
                    LoadProducts();
                }
                else if (datagrid.SelectedItem is Arrivals arrival)
                {
                    ShowResult(storageService.DeleteArrival(arrival));
                    LoadArrivals();
                }
                else if (datagrid.SelectedItem is Expenses expense)
                {
                    ShowResult(storageService.DeleteExpense(expense));
                    LoadExpenses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления данных: " + ex.Message);
            }
        }

        private void changeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите объект для редактирования.");
                return;
            }

            if (datagrid.SelectedItem is Products product)
            {
                EditProductWindow editProductWindow = new EditProductWindow(product);
                if (editProductWindow.ShowDialog() == true)
                    LoadProducts();
            }
            else if (datagrid.SelectedItem is Arrivals arrival)
            {
                EditArrivalWindow editArrivalWindow = new EditArrivalWindow(arrival);
                if (editArrivalWindow.ShowDialog() == true)
                    LoadArrivals();
            }
            else if (datagrid.SelectedItem is Expenses expense)
            {
                EditExpensesWindow editExpensesWindow = new EditExpensesWindow(expense);
                if (editExpensesWindow.ShowDialog() == true)
                    LoadExpenses();
            }
        }

        private void arriveDataBtn_Click(object sender, RoutedEventArgs e)
        {
            IncomeReportWindow incomeReportWindow = new IncomeReportWindow();
            incomeReportWindow.ShowDialog();
        }

        private void expensesDataBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpensesReportWindiw expensesReportWindiw = new ExpensesReportWindiw();
            expensesReportWindiw.ShowDialog();
        }

        private void LoadProducts()
        {
            LoadGrid(() => storageService.GetProducts());
        }

        private void LoadArrivals()
        {
            LoadGrid(() => storageService.GetArrivals());
        }

        private void LoadExpenses()
        {
            LoadGrid(() => storageService.GetExpenses());
        }

        private void LoadGrid<T>(Func<List<T>> loadData)
        {
            try
            {
                datagrid.ItemsSource = null;
                datagrid.ItemsSource = loadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private static void ShowResult(OperationResult result)
        {
            MessageBox.Show(result.Message);
        }
    }
}
