using System;
using System.Windows;
using testSrorage.Application;
using testSrorage.Application.Interfaces;
using testSrorage.Domain;
using testSrorage.Infrastructure;

namespace testSrorage
{
    public partial class App : System.Windows.Application
    {
        public IStorageService StorageService { get; private set; }

        public static IStorageService CurrentStorageService
        {
            get { return ((App)Current).StorageService; }
        }

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

        private void ConfigureServices()
        {
            DbConnectionFactory connectionFactory = new DbConnectionFactory();
            DatabaseInitializer initializer = new DatabaseInitializer(connectionFactory);
            initializer.Initialize(typeof(Product).Assembly);

            StorageService = new StorageService(
                new GenericRepository<Product>(connectionFactory),
                new GenericRepository<Arrival>(connectionFactory),
                new GenericRepository<Expense>(connectionFactory));
        }
    }
}
