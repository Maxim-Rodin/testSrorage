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
    // Главное окно приложения.
    // Содержит обработчики кнопок, логику загрузки данных в `datagrid`
    // и открытия/обновления окон добавления/редактирования/отчётов.
    public partial class MainWindow : Window
    {
        private readonly IStorageService storageService = App.CurrentStorageService;
        private Type currentViewType;

        // Конструктор окна. Инициализирует компоненты интерфейса.
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик кнопки проверки подключения к БД. Показывает результат проверки.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowResult(storageService.CheckConnection());
        }

        // Обработчик кнопки загрузки списка товаров.
        private void checkBtn1_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        // Обработчик кнопки открытия окна добавления прихода.
        private void arrivalBtn_Click(object sender, RoutedEventArgs e)
        {
            AddArrivalWindow addArrivalWindow = new AddArrivalWindow();
            addArrivalWindow.ShowDialog();
            RefreshCurrentView();
        }

        // Обработчик кнопки загрузки приходов.
        private void checkArriveBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadArrivals();
        }

        // Обработчик кнопки загрузки расходов.
        private void checkExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadExpenses();
        }

        // Обработчик кнопки открытия окна добавления расхода.
        private void addExpenesBtn_Click(object sender, RoutedEventArgs e)
        {
            AddExpensesWindow addExpensesWindow = new AddExpensesWindow();
            addExpensesWindow.ShowDialog();
            RefreshCurrentView();
        }

        // Обработчик кнопки удаления выбранного элемента из таблицы.
        // Получает Id выбранного объекта и вызывает универсальный метод Delete сервиса.
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

        // Обработчик кнопки изменения выбранного элемента.
        // Открывает соответствующее окно редактирования для выбранного объекта.
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

        // Обработчик кнопки открытия окна отчёта по приходам.
        private void arriveDataBtn_Click(object sender, RoutedEventArgs e)
        {
            IncomeReportWindow incomeReportWindow = new IncomeReportWindow();
            incomeReportWindow.ShowDialog();
        }

        // Обработчик кнопки открытия окна отчёта по расходам.
        private void expensesDataBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpensesReportWindiw expensesReportWindiw = new ExpensesReportWindiw();
            expensesReportWindiw.ShowDialog();
        }

        // Вспомогательный метод загрузки списка товаров.
        private void LoadProducts()
        {
            LoadGridByType(typeof(Product));
        }

        // Вспомогательный метод загрузки списка приходов.
        private void LoadArrivals()
        {
            LoadGridByType(typeof(Arrival));
        }

        // Вспомогательный метод загрузки списка расходов.
        private void LoadExpenses()
        {
            LoadGridByType(typeof(Expense));
        }

        // Универсальный метод загрузки данных для заданного типа.
        // Использует рефлексию для вызова `IStorageService.GetAll<T>()` и заполняет `datagrid`.
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

        // Обновляет текущий вид (повторно загружает данные для текущего типа).
        private void RefreshCurrentView()
        {
            if (currentViewType != null)
                LoadGridByType(currentViewType);
        }

        // Открывает окно редактирования для переданного объекта.
        // Ищет тип окна `Edit{TypeName}Window`, пытается найти подходящий конструктор
        // или установить `DataContext`, вызывает `ShowDialog` и при успехе обновляет вид.
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

        // Вспомогательный обобщённый метод загрузки данных в датагрид с обработкой исключений.
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

        // Вспомогательный метод отображения результата операции (сообщение пользователю).
        private static void ShowResult(OperationResult result)
        {
            MessageBox.Show(result.Message);
        }
    }
}
