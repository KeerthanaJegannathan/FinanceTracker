using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Data
{
    public interface ITransactionRepository
    {

        public IEnumerable<Transaction> GetAll();

        Transaction? GetById(int id);    
        void Add(Transaction transaction);

        void Update(Transaction transaction);   
        void Delete(int id);    
    }
}
