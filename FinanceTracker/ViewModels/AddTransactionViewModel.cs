using FinanceTracker.Commands;
using FinanceTracker.Data;
using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace FinanceTracker.ViewModels
{
    public class AddTransactionViewModel : BaseViewModel
    {

        private readonly ITransactionRepository _transactionrepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly Action _onSaved;
        private readonly Transaction? _existingTransaction;

        //Fields
        private decimal _amount;
        private Category? _selectedCategory;
        private DateTime _date=DateTime.Now;    
        private string _note = string.Empty;
        private TransactionType _type = TransactionType.Expense;
        private string _validationMessage= string.Empty; 

        //Constructor

        public AddTransactionViewModel(ITransactionRepository transactionrepository, ICategoryRepository categoryrepository
            , Action onSaved,  Transaction? existingTransaction = null)
        {
            transactionrepository = _transactionrepository;
            categoryrepository = _categoryRepository;
            _onSaved = onSaved;
            existingTransaction=_existingTransaction;   

            Categories = new ObservableCollection<Category>();
            TransactionTypes = new ObservableCollection<TransactionType>
            {TransactionType.Income, TransactionType.Expense };

            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());


            if(_existingTransaction != null)
            {
                PrePopulateForEdit(_existingTransaction);
            }
            LoadCategories();

        }


        //Collections

        public ObservableCollection<Category>Categories { get; }
        public ObservableCollection<TransactionType> TransactionTypes { get; }

        //Properties

        public string Title => _existingTransaction != null
            ? "Edit Transaction"
            : "Add Transaction";

        public string SaveButtonLabel => _existingTransaction != null
            ? "Save Changes"
            : "Add Transaction";
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    ValidationMessage = string.Empty;
                }
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    ValidationMessage= string.Empty;    
                }
            }
        }
        public DateTime Date
        {
            get => _date; 
            set
            {
                SetProperty(ref _date, value);  
            }
        }

        public string Note
        {
            get {  return _note; }
            set { SetProperty(ref _note, value); }
        }
        public TransactionType Type
        {
            get => _type;
            set
            {
                if(SetProperty(ref _type, value))
                {
                    LoadCategories();
                }
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);  

        }

        //Commands
        
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        //PRIVATE METHODS

        private void PrePopulateForEdit(Transaction transaction)
        {
            _amount = transaction.Amount;
            _date = transaction.Date;
            _note = transaction.Note;
            _type = transaction.Type;
        }

        private void LoadCategories()
        {
            Categories.Clear();

            // Mapping TransactionType to CategoryType for the repository filter
            var categoryType = Type == TransactionType.Income
                ? CategoryType.Income
                : CategoryType.Expense;

            var loaded = _categoryRepository.GetByType(categoryType);
            foreach (var category in loaded) 
                Categories.Add(category);

            
            SelectedCategory = _existingTransaction!=null ? 
                Categories.FirstOrDefault(c => c.Id == _existingTransaction.CategoryId) :
                Categories.FirstOrDefault();
        }

        private bool CanSave() => Amount > 0 && SelectedCategory != null;

        private void Save()
        {
            if (!CanSave())
            {
                ValidationMessage = "Please enter a valida amount and select a category";
                return;
            }
            if (_existingTransaction != null)
            {
                _existingTransaction.Amount = Amount;
                _existingTransaction.Note = Note;
                _existingTransaction.CategoryId = SelectedCategory.Id;
                _existingTransaction.Type = Type;
                _existingTransaction.Date = Date;

                _transactionrepository.Update(_existingTransaction);
            }
            else
            {
                _transactionrepository.Add(new Transaction
                {
                    Amount = Amount,
                    CategoryId = SelectedCategory!.Id,
                    Note = Note,
                    Type = Type,
                    Date = Date
                });
            }
            
            _onSaved.Invoke();
            RequestClose?.Invoke(); 
        }

        private void Cancel()
        {

            RequestClose?.Invoke();
        }

        public event Action? RequestClose;
    }
}
