using FinanceTracker.Commands;
using FinanceTracker.Data;
using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace FinanceTracker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ITransactionRepository _transactionRepository;
        private ICategoryRepository _categoryRepository;

        private string _selectedFilterCategory = "All";
        private Transaction? _selectedTransaction;
        private bool _isLoading;

        public MainViewModel(ITransactionRepository transactionRepo, ICategoryRepository categoryRepo)
        {
            _transactionRepository = transactionRepo;
            _categoryRepository = categoryRepo;
            Transactions = new ObservableCollection<Transaction>(); 
            FilteredTransactions = new ObservableCollection<Transaction>();
            FilterCategories = new ObservableCollection<string>();

            AddCommand = new RelayCommand(_ => OpenAddDialog());
            EditCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedTransaction != null);
            DeleteCommand = new RelayCommand(_=> DeleteSelected(), _=> SelectedTransaction != null);
            RefreshCommand = new RelayCommand(_ => LoadTransactions());

            LoadTransactions();

        }


        //Properties

        /// <summary>Sum of all Income transactions.</summary>
        public decimal TotalIncome
            => Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);

        //SUM OF ALL EXPENSE TRANSACTIONS.
        public decimal TotalExpenses
            => Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        //TotalIncome minus TotalExpenses
        public decimal NetBalance => TotalIncome - TotalExpenses;

        /// <summary>
        /// True when spending exceeds income.
        /// </summary>
        public bool IsOverBudget => NetBalance < 0;

        public string SelectedFilterCategory
        {
            get => _selectedFilterCategory;
            set
            {
                if (SetProperty(ref _selectedFilterCategory, value))
                    ApplyFilter();
            }
        }

 
        public Transaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Private Methods

        private void LoadTransactions()
        {
            IsLoading = true;

            Transactions.Clear();
            foreach (var t in _transactionRepository.GetAll())
                Transactions.Add(t);

            RebuildFilterCategories();
            ApplyFilter();
            RefreshSummary();

            IsLoading = false;
        }

        private void RefreshSummary()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(NetBalance));
            OnPropertyChanged(nameof(IsOverBudget));
        }

        private void RebuildFilterCategories()
        {
            var current = SelectedFilterCategory;

            FilterCategories.Clear();
            FilterCategories.Add("All");
            foreach (var cat in _categoryRepository.GetAll())
                FilterCategories.Add(cat.Name);

            SelectedFilterCategory = FilterCategories.Contains(current) ? current : "All";
        }


        private void ApplyFilter()
        {
            FilteredTransactions.Clear();

            IEnumerable<Transaction> filtered = SelectedFilterCategory == "All"
                ? Transactions
                : Transactions.Where(t => t.CategoryName == SelectedFilterCategory);

            foreach (var t in filtered)
                FilteredTransactions.Add(t);
        }


        private void OpenAddDialog()
        {
            var vm = new AddTransactionViewModel(
                _transactionRepository,
                _categoryRepository,
                onSaved: () => LoadTransactions());

            var dialog = new Views.AddTransactionView
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        private void DeleteSelected()
        {

            if (SelectedTransaction == null) return;

            _transactionRepository.Delete(SelectedTransaction.Id);
            Transactions.Remove(SelectedTransaction);
            SelectedTransaction = null;

            ApplyFilter();
            RefreshSummary();
        }

        private void OpenEditDialog()
        {
            if (SelectedTransaction == null) return;

            var vm = new AddTransactionViewModel(
                _transactionRepository,
                _categoryRepository,
                onSaved: () => LoadTransactions(),
                existingTransaction: SelectedTransaction);

            var dialog = new Views.AddTransactionView
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }
        //Collections

        public ObservableCollection<Transaction> Transactions { get; }
        public ObservableCollection<string> FilterCategories { get; }

        public ObservableCollection<Transaction> FilteredTransactions {  get; }

        //Commands

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
    }
}
