using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        //ForeignKey => Category.Id
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryColor { get; set; } = "#95A5A6";

        public DateTime Date { get; set; }

        public string Note { get; set; }

        public TransactionType Type { get; set; }

        public enum TransactionType
        {
            Income = 0,
            Expense = 1
        }
    }

}
