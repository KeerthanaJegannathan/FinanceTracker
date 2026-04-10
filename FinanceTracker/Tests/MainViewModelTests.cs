using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FinanceTracker.Tests
{
    public class MainViewModelTests
    {

        private class MockTransactionRepository : ITransactionRepository
        {
            private readonly List<Transaction> _data;
            public MockTransactionRepository(List<Transaction> data) => _data = data;
            public IEnumerable<Transaction> GetAll() => _data;
            public Transaction? GetById(int id) => _data.Find(t => t.Id == id);
            public void Add(Transaction t) => _data.Add(t);
            public void Update(Transaction t) { }
            public void Delete(int id) => _data.RemoveAll(t => t.Id == id);
        }

        private class MockCategoryRepository : ICategoryRepository
        {
            private static readonly List<Category> _defaults = new()
            {
                new() { Id = 1, Name = "Salary",    Type = CategoryType.Income  },
                new() { Id = 2, Name = "Food",      Type = CategoryType.Expense },
                new() { Id = 3, Name = "Transport", Type = CategoryType.Expense },
                new() { Id = 4, Name = "Other",     Type = CategoryType.Both    }
            };
            public IEnumerable<Category> GetAll() => _defaults;
            public IEnumerable<Category> GetByType(CategoryType type) => _defaults;
            public Category? GetById(int id) => null;
            public void Add(Category c) { }
            public void Update(Category c) { }
            public void Delete(int id) { }
        }


        private static MainViewModel CreateVm(List<Transaction> transactions) =>
            new(new MockTransactionRepository(transactions), new MockCategoryRepository());


        [Fact]
        public void TotalIncome_SumsIncomeTransactionsOnly()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 2000m, Type = TransactionType.Income,  CategoryName = "Salary" },
                new() { Id = 2, Amount = 50m,   Type = TransactionType.Expense, CategoryName = "Food"   },
                new() { Id = 3, Amount = 500m,  Type = TransactionType.Income,  CategoryName = "Other"  }
            });

            Assert.Equal(2500m, vm.TotalIncome);
        }

        [Fact]
        public void TotalExpenses_SumsExpenseTransactionsOnly()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 2000m, Type = TransactionType.Income,  CategoryName = "Salary" },
                new() { Id = 2, Amount = 50m,   Type = TransactionType.Expense, CategoryName = "Food"   },
                new() { Id = 3, Amount = 200m,  Type = TransactionType.Expense, CategoryName = "Transport" }
            });

            Assert.Equal(250m, vm.TotalExpenses);
        }

        [Fact]
        public void NetBalance_IsIncomeMinus_Expenses()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 3000m, Type = TransactionType.Income,  CategoryName = "Salary" },
                new() { Id = 2, Amount = 500m,  Type = TransactionType.Expense, CategoryName = "Food"   }
            });

            Assert.Equal(2500m, vm.NetBalance);
        }

        [Fact]
        public void IsOverBudget_TrueWhenExpensesExceedIncome()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 100m,  Type = TransactionType.Income,  CategoryName = "Salary" },
                new() { Id = 2, Amount = 500m,  Type = TransactionType.Expense, CategoryName = "Food"   }
            });

            Assert.True(vm.IsOverBudget);
        }

        [Fact]
        public void FilteredTransactions_UpdatesWhenCategoryChanges()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 10m, Type = TransactionType.Expense, CategoryName = "Food"      },
                new() { Id = 2, Amount = 20m, Type = TransactionType.Expense, CategoryName = "Transport" },
                new() { Id = 3, Amount = 15m, Type = TransactionType.Expense, CategoryName = "Food"      }
            });

            vm.SelectedFilterCategory = "Food";

            Assert.Equal(2, vm.FilteredTransactions.Count);
            Assert.All(vm.FilteredTransactions, t => Assert.Equal("Food", t.CategoryName));
        }

        [Fact]
        public void FilteredTransactions_ShowsAll_WhenCategoryIsAll()
        {
            var vm = CreateVm(new List<Transaction>
            {
                new() { Id = 1, Amount = 10m, Type = TransactionType.Expense, CategoryName = "Food"      },
                new() { Id = 2, Amount = 20m, Type = TransactionType.Expense, CategoryName = "Transport" }
            });

            vm.SelectedFilterCategory = "All";

            Assert.Equal(2, vm.FilteredTransactions.Count);
        }
    }
}
