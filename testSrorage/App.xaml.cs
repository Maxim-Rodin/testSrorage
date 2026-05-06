using System;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;
using testSrorage.Infrastructure;

namespace testSrorage
{
    // Точка входа приложения WPF. Конфигурирует сервисы и запускает главное окно.
    public partial class App : System.Windows.Application
    {
        public IStorageService StorageService { get; private set; }

        // Удобный статический доступ к текущему сервису хранения.
        public static IStorageService CurrentStorageService
        {
            get { return ((App)Current).StorageService; }
        }
                
        // Выполняется при старте приложения — конфигурирование сервисов и открытие главного окна.
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ConfigureServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось запустить приложение: " + ex.Message);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            MainWindow window = new MainWindow();
            MainWindow = window;
            window.Show();
        }

        // Создаёт фабрики и сервисы приложения (инициализация БД, репозиториев и сервисов).
        private void ConfigureServices()
        {
            DbConnectionFactory connectionFactory = new DbConnectionFactory();
            DatabaseInitializer initializer = new DatabaseInitializer(connectionFactory);
            initializer.Initialize(typeof(Product).Assembly);

            IRepositoryFactory repoFactory = new RepositoryFactory(connectionFactory);
            StorageService = new StorageService(repoFactory); // Использует конструктор StorageService(IRepositoryFactory)
        }
    }
}
