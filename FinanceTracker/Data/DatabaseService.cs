using FinanceTracker.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;

namespace FinanceTracker.Data
{
    public class DatabaseService : ITransactionRepository, ICategoryRepository
    {

        private readonly string _connectionString;

        //Constructor
        public DatabaseService(string dbPath= "Finance.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitialiseDatabase();
        }

        private void InitialiseDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            CreatecategoriesTable(connection);
            CreateTransactionTable(connection);
            SeedDefaultCategories(connection);
        }

        private static void CreatecategoriesTable(SqliteConnection connection)
        {
           var command= connection.CreateCommand();
            command.CommandText = @"
              CREATE TABLE IF NOT EXISTS Categories(
              Id INTEGER PRIMARY KEY AUTO INCREMENT,
              Name TEXT NOT NULL UNIQUE,
              Type INTEGER NOT NULL DEFAULT 2,
              Color TEXT NOT NULL DEFAULT '#95A5A6',
              IconName TEXT NOT NULL DEFAULT '',
              IsDefault INTEGER DEFAULT 1,
              CreatedAt TEXT NOT NULL DEFAULT (date('now'))
              );
            "; 
            command.ExecuteNonQuery();  
        }
        private static void CreateTransactionTable(SqliteConnection connection)
        {
            var command= connection.CreateCommand();
            command.CommandText = @"
               CREATE TABLE IF NOT EXISTS Transactions(
                 Id INTEGER PRIMARY KEY AUTO INCREMENT,
                 Amount REAL NOT NULL,
                 CategoryId INTEGER NOT NULL,
                 Date TEXT NOT NULL,
                 Note TEXT NOT NULL '',
                 Type INTEGER NOT NULL,
                
                 FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
                        ON DELETE RESTRICT
             );";
            command.ExecuteNonQuery();
            
        }
        private static void SeedDefaultCategories(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO Categories (Name, Type, Colour, IconName, IsDefault, CreatedAt) VALUES
                --  Name              Type  Colour      Icon            IsDefault  CreatedAt
                ('Salary',           0,    '#27AE60',  'salary',        1,        date('now')),
                ('Freelance',        0,    '#2ECC71',  'freelance',     1,        date('now')),
                ('Investment',       0,    '#1ABC9C',  'investment',    1,        date('now')),
                ('Food',             1,    '#E67E22',  'food',          1,        date('now')),
                ('Transport',        1,    '#3498DB',  'transport',     1,        date('now')),
                ('Rent',             1,    '#E74C3C',  'rent',          1,        date('now')),
                ('Entertainment',    1,    '#9B59B6',  'entertainment', 1,        date('now')),
                ('Health',           1,    '#EC407A',  'health',        1,        date('now')),
                ('Shopping',         1,    '#FF7043',  'shopping',      1,        date('now')),
                ('Utilities',        1,    '#5C6BC0',  'utilities',     1,        date('now')),
                ('Other',            2,    '#95A5A6',  'other',         1,        date('now'));";
            command.ExecuteNonQuery();
        }
       

        //CATEGORY REPOSITORY -ICATEGORYREPOSITORY

        public void Add(Category category)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public Category? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetByType(CategoryType type)
        {
            throw new NotImplementedException();
        }

        public void Update(Category category)
        {
            throw new NotImplementedException();
        }


        //TRANSACTION REPOSITORY- ITRANSTIONREPOSITORY

        void ITransactionRepository.Add(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        void ITransactionRepository.Delete(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Transaction> ITransactionRepository.GetAll()
        {
            throw new NotImplementedException();
        }

        Transaction? ITransactionRepository.GetById(int id)
        {
            throw new NotImplementedException();
        }

        void ITransactionRepository.Update(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
