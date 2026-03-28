using FinanceTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceTracker.Views
{
    /// <summary>
    /// Interaction logic for AddTransactionView.xaml
    /// </summary>
    public partial class AddTransactionView : Window
    {
        public AddTransactionView()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is AddTransactionViewModel vm)
                {
                    // When the ViewModel raises RequestClose, close this window.
                    vm.RequestClose += () => this.Close();
                }
            };
        }
    }
}
