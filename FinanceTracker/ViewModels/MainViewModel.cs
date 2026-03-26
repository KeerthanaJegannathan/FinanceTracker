using FinanceTracker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.ViewModels
{
    public class MainViewModel
    {
        private ITransactionRepository transactionRepo;
        private ICategoryRepository categoryRepo;

        public MainViewModel(ITransactionRepository transactionRepo, ICategoryRepository categoryRepo)
        {
            this.transactionRepo = transactionRepo;
            this.categoryRepo = categoryRepo;
        }
    }
}
