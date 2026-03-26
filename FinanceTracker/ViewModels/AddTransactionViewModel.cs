using FinanceTracker.Data;
using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;

namespace FinanceTracker.ViewModels
{
    public class AddTransactionViewModel : BaseViewModel
    {

        private readonly ITransactionRepository _transactionrepository;
        private readonly ICategoryRepository _categoryrepository;
        private readonly Action _onSaved;

        //Fields
        private decimal _amount;
        private Category? _selectedCategory;
        private DateTime _date=DateTime.Now;    
        private string _note = string.Empty;
        private TransactionType _type = TransactionType.Expense;
        private string validationMessage= string.Empty; 

        //Constructor

        public AddTransactionViewModel(ITransactionRepository transactionrepository, ICategoryRepository categoryre
            ,Action onSaved)
        {
            transactionrepository = _transactionrepository;
            _categoryrepository = _categoryrepository;
            _onSaved = onSaved;
            Categories = new ObservableCollection<Category>();
            TransactionTypes = new ObservableCollection<TransactionType>
            {TransactionType.Income, TransactionType.Expense };
        }

        public ObservableCollection<Category>Categories { get; }
        public ObservableCollection<TransactionType> TransactionTypes { get; }  
    }
}
