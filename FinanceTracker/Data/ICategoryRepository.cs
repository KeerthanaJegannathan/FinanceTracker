using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Data
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        IEnumerable<Category> GetByType(CategoryType type);
        Category? GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}
