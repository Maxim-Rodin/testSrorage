using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;

namespace testSrorage
{
    public partial class MainWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private Type currentViewType;

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
            RefreshCurrentView();
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
            RefreshCurrentView();
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

                object selected = datagrid.SelectedItem;
                Type itemType = selected.GetType();

                PropertyInfo idProp = itemType.GetProperty(nameof(BaseEntity.Id));
                if (idProp == null)
                {
                    MessageBox.Show("У выбранного объекта нет свойства Id.");
                    return;
                }

                int id = (int)idProp.GetValue(selected, null);

                MethodInfo deleteMethod = storageService.GetType()
                    .GetMethod(nameof(IStorageService.Delete))
                    .MakeGenericMethod(itemType);

                var result = (OperationResult)deleteMethod.Invoke(storageService, new object[] { id });

                ShowResult(result);
                RefreshCurrentView();
            }
            catch (TargetInvocationException tie) // unwrap inner exception for clarity
            {
                MessageBox.Show("Ошибка удаления данных: " + (tie.InnerException?.Message ?? tie.Message));
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

            object selected = datagrid.SelectedItem;
            OpenEditWindowForItem(selected);
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
            LoadGridByType(typeof(Product));
        }

        private void LoadArrivals()
        {
            LoadGridByType(typeof(Arrival));
        }

        private void LoadExpenses()
        {
            LoadGridByType(typeof(Expense));
        }

        private void LoadGridByType(Type type)
        {
            try
            {
                datagrid.ItemsSource = null;

                MethodInfo getAll = storageService.GetType().GetMethod(nameof(IStorageService.GetAll))
                    .MakeGenericMethod(type);

                var list = getAll.Invoke(storageService, null);

                datagrid.ItemsSource = (IEnumerable)list;
                currentViewType = type;
            }
            catch (TargetInvocationException tie)
            {
                MessageBox.Show("Ошибка загрузки данных: " + (tie.InnerException?.Message ?? tie.Message));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void RefreshCurrentView()
        {
            if (currentViewType != null)
                LoadGridByType(currentViewType);
        }

        private void OpenEditWindowForItem(object item)
        {
            Type itemType = item.GetType();
            string windowName = "Edit" + itemType.Name + "Window";

            Type windowType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => string.Equals(t.Name, windowName, StringComparison.OrdinalIgnoreCase));

            if (windowType == null)
            {
                MessageBox.Show("Окно редактирования для типа " + itemType.Name + " не найдено.");
                return;
            }

            // Найти конструктор, принимающий объект нужного типа
            ConstructorInfo ctor = windowType.GetConstructors()
                .FirstOrDefault(c =>
                {
                    var ps = c.GetParameters();
                    return ps.Length == 1 && ps[0].ParameterType.IsAssignableFrom(itemType);
                });

            object windowInstance;
            if (ctor != null)
            {
                windowInstance = ctor.Invoke(new object[] { item });
            }
            else
            {
                // если конструктора с параметром нет — попробуем без параметров и установить DataContext (если нужно)
                ctor = windowType.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    MessageBox.Show("Не найден подходящий конструктор для окна редактирования " + windowName);
                    return;
                }
                windowInstance = ctor.Invoke(null);

                // попытка установить DataContext = item, если есть такое свойство
                PropertyInfo dc = windowType.GetProperty("DataContext");
                if (dc != null && dc.CanWrite)
                    dc.SetValue(windowInstance, item, null);
            }

            // Показать окно (вызов ShowDialog)
            MethodInfo showDialog = windowType.GetMethod("ShowDialog", Type.EmptyTypes);
            if (showDialog == null)
            {
                MessageBox.Show("Окно редактирования не поддерживает ShowDialog.");
                return;
            }

            var res = showDialog.Invoke(windowInstance, null) as bool?;
            if (res == true)
                RefreshCurrentView();
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
