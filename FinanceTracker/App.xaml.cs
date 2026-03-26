using FinanceTracker.Data;
using FinanceTracker.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var db = new DatabaseService("finance.db");

            ITransactionRepository transactionRepo = db;
            ICategoryRepository categoryRepo = db;

            var mainViewModel = new MainViewModel(transactionRepo, categoryRepo);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }

}
