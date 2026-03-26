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
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command= connection.CreateCommand();
            command.CommandText = @"
                 INSERT INTO Categories( Name, Type, Color, IconName, IsDefault, CreatedAt)
                  VALUES ($name, $type, $colour, $iconName, 0, date('now'));";
            command.Parameters.AddWithValue("$name", category.Name);
            command.Parameters.AddWithValue("$type", (int)category.Type);
            command.Parameters.AddWithValue("$colour", category.Color);
            command.Parameters.AddWithValue("$iconName", category.IconName);

            command.ExecuteNonQuery();
        }

     
        /// <summary>Returns all categories ordered alphabetically</summary>
        public IEnumerable<Category> GetAll()
        {
            var categories = new List<Category>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Type, Colour, IconName, IsDefault, CreatedAt
                FROM   Categories
                ORDER  BY Name ASC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
                categories.Add(MapCategoryFromReader(reader));

            return categories;
        }

        public Category? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Type, Colour, IconName, IsDefault, CreatedAt
                FROM   Categories
                WHERE  Id = $id;";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapCategoryFromReader(reader) : null;
        }

        //Return Categories filtered by type
        public IEnumerable<Category> GetByType(CategoryType type)
        {
            var categories = new List<Category>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            // Return categories that match the type OR are marked as Both (2)
            command.CommandText = @"
                SELECT Id, Name, Type, Colour, IconName, IsDefault, CreatedAt
                FROM   Categories
                WHERE  Type = $type OR Type = 2
                ORDER  BY Name ASC;";
            command.Parameters.AddWithValue("$type", (int)type);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                categories.Add(MapCategoryFromReader(reader));

            return categories;
        }

        //Update category by id
        public void Update(Category category)
        {
            using var connection= new SqliteConnection(_connectionString);
            connection.Open();

            var command= connection.CreateCommand();
            command.CommandText = @"
              UPDATE Categories
               SET Name= $name,
                   Type= $type,
                   Color= $color,
                   IconName= $iconName
              WHERE Id= $id;";

            command.Parameters.AddWithValue("$id", category.Id);
            command.Parameters.AddWithValue("$name", category.Name);
            command.Parameters.AddWithValue("$type", (int) category.Type);
            command.Parameters.AddWithValue("$Color", category.Color);
            command.Parameters.AddWithValue("iconName", category.IconName);

            command.ExecuteNonQuery();  

        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Categories WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }


        //TRANSACTION REPOSITORY- ITRANSTIONREPOSITORY1z1



        /// <summary>
        /// Returns all transactions with CategoryName and CategoryColour
        /// </summary>
        IEnumerable<Transaction> ITransactionRepository.GetAll()
        {
            var transactions = new List<Transaction>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.Id,
                       t.Amount,
                       t.CategoryId,
                       c.Name   AS CategoryName,
                       c.Colour AS CategoryColour,
                       t.Date,
                       t.Note,
                       t.Type
                FROM   Transactions t
                JOIN   Categories   c ON t.CategoryId = c.Id
                ORDER  BY t.Date DESC;";

            using var reader = command.ExecuteReader();
            while (reader.Read())
                transactions.Add(MapTransactionFromReader(reader));

            return transactions;
        }

        Transaction? ITransactionRepository.GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.Id,
                       t.Amount,
                       t.CategoryId,
                       c.Name   AS CategoryName,
                       c.Colour AS CategoryColour,
                       t.Date,
                       t.Note,
                       t.Type
                FROM   Transactions t
                JOIN   Categories   c ON t.CategoryId = c.Id
                WHERE  t.Id = $id;";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapTransactionFromReader(reader) : null;
        }


        void ITransactionRepository.Add(Transaction transaction)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Transactions (Amount, CategoryId, Date, Note, Type)
                VALUES ($amount, $categoryId, $date, $note, $type);";

            command.Parameters.AddWithValue("$amount", transaction.Amount);
            command.Parameters.AddWithValue("$categoryId", transaction.CategoryId);
            command.Parameters.AddWithValue("$date", transaction.Date.ToString("o")); // ISO 8601
            command.Parameters.AddWithValue("$note", transaction.Note ?? string.Empty);
            command.Parameters.AddWithValue("$type", (int)transaction.Type);

            command.ExecuteNonQuery();
        }

        void ITransactionRepository.Update(Transaction transaction)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Transactions
                SET Amount     = $amount,
                    CategoryId = $categoryId,
                    Date       = $date,
                    Note       = $note,
                    Type       = $type
                WHERE Id = $id;";

            command.Parameters.AddWithValue("$id", transaction.Id);
            command.Parameters.AddWithValue("$amount", transaction.Amount);
            command.Parameters.AddWithValue("$categoryId", transaction.CategoryId);
            command.Parameters.AddWithValue("$date", transaction.Date.ToString("o"));
            command.Parameters.AddWithValue("$note", transaction.Note ?? string.Empty);
            command.Parameters.AddWithValue("$type", (int)transaction.Type);

            command.ExecuteNonQuery();
        }

        void ITransactionRepository.Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Transactions WHERE Id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }


        private static Category MapCategoryFromReader(SqliteDataReader reader)
        {
            return new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = (CategoryType)reader.GetInt32(2),
                Color = reader.GetString(3),
                IconName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                IsDefault = reader.GetInt32(5) == 1,
                CreatedAt = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
            };
        }


        private static Transaction MapTransactionFromReader(SqliteDataReader reader)
        {
            return new Transaction
            {
                Id = reader.GetInt32(0),
                Amount = reader.GetDecimal(1),
                CategoryId = reader.GetInt32(2),
                CategoryName = reader.GetString(3),
                CategoryColor = reader.GetString(4),
                Date = DateTime.Parse(reader.GetString(5)),
                Note = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Type = (TransactionType)reader.GetInt32(7)
            };
        }
    }
}
