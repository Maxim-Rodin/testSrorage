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
using System.Windows.Navigation;
using System.Windows.Shapes;
using testSrorage.классы;

namespace testSrorage
{
    
    public partial class MainWindow : Window
    {
        
        

        public MainWindow()
        {

            InitializeComponent();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DB databa = new DB();
            databa.OpenConnection();
        }

        private void checkBtn1_Click(object sender, RoutedEventArgs e)
        {
           DBManager maneger = new DBManager();
            datagrid.ItemsSource = null;
            List<Products> products = maneger.GetAllProducts();
            if (products != null)
            {
                datagrid.ItemsSource = products;
            }

        }

        private void arrivalBtn_Click(object sender, RoutedEventArgs e)
        {
            AddArrivalWindow addArrivalWindow = new AddArrivalWindow();
            addArrivalWindow.Show();
        }

        private void checkArriveBtn_Click(object sender, RoutedEventArgs e)
        {
            DBManager maneger = new DBManager();
            datagrid.ItemsSource = null;
            
            List<Arrivals> arrivals = maneger.GetAllArrivals();
            if(arrivals != null)
            {
                datagrid.ItemsSource = arrivals;

            }
            
        }

        private void checkExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            DBManager maneger = new DBManager();
            datagrid.ItemsSource = null;

            List<Expenses> expenseses = maneger.GetAllExpenses();
            if (expenseses != null)
            {
                datagrid.ItemsSource = expenseses;

            }

        }

        private void addExpenesBtn_Click(object sender, RoutedEventArgs e)
        {
            AddExpensesWindow addExpensesWindow = new AddExpensesWindow();
            addExpensesWindow.Show();
        }

        private void deletBtn_Click(object sender, RoutedEventArgs e)
        {
            DBManager maneger = new DBManager();

            var selectedItem  = datagrid.SelectedItem;

            if (selectedItem is Products product) 
            {
                maneger.DeleteProduct(product);
                datagrid.ItemsSource = null;
                List<Products> products = maneger.GetAllProducts();
                if (products != null)
                {
                    datagrid.ItemsSource = products;
                }
            }
            else if (selectedItem is Arrivals arrival )
            {
                maneger.DeleteArrival(arrival.IdArrivals);
                datagrid.ItemsSource= null;
                List<Arrivals> arrivals = maneger.GetAllArrivals();
                if (arrivals != null)
                {
                    datagrid.ItemsSource = arrivals;

                }


            }
            else if(selectedItem is Expenses expense ) 
            { 
                maneger.DeleteExpense(expense.IdExpenses);
                datagrid.ItemsSource=null;
                List<Expenses> expenseses = maneger.GetAllExpenses();
                if (expenseses != null)
                {
                    datagrid.ItemsSource = expenseses;

                }


            }



        }

        private void changeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItems == null)
            {
                MessageBox.Show("Выберите обьект для редактирования");
                return;
            }
            else
            {
                var selectedItem = datagrid.SelectedItem;
                if (selectedItem is Products product)
                {
                    EditProductWindow editProductWindow = new EditProductWindow(product);
                    editProductWindow.ShowDialog();
                }
                else if (selectedItem is Arrivals arrival)
                {
                    EditArrivalWindow editArrivalWindow = new EditArrivalWindow(arrival);
                    editArrivalWindow.ShowDialog();
                }
                else if (selectedItem is Expenses expense) 
                {
                    EditExpensesWindow editExpensesWindow = new EditExpensesWindow(expense);
                    editExpensesWindow.ShowDialog();
                }
              
                
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
    }
}
