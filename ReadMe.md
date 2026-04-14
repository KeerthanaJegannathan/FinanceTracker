# Personal Finance Tracker

A desktop finance management application built with **WPF**, **C#**, and strict **MVVM architecture**.

![.NET](https://img.shields.io/badge/.NET-6.0+-512BD4?style=flat&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-C%23-239120?style=flat&logo=csharp)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat&logo=sqlite)
![xUnit](https://img.shields.io/badge/Tests-xUnit-512BD4?style=flat)


## Features

* Add, edit, and delete income and expense transactions
* Filter transactions by category in real-time
* Live summary cards — Total Income, Total Expenses, Net Balance
* Balance card turns red automatically when spending exceeds income
* Persistent local storage via SQLite (no server needed)
* Empty state messaging when no transactions exist
* Edit and Delete buttons auto-disable when no row is selected


##  Architecture

This project follows strict **MVVM (Model-View-ViewModel)** architecture.

FinanceTracker/
├── Commands/
│   └── RelayCommand.cs             Reusable ICommand — no event handlers in code-behind
│
├── Data/
│   ├── ITransactionRepository.cs   Interface for transaction data access
│   ├── ICategoryRepository.cs      Interface for category data access
│   └── DatabaseService.cs          SQLite implementation of both interfaces
│
├── Models/
│   ├── Transaction.cs              Plain transaction model (no UI logic)
│   └── Category.cs                 Plain category model (no UI logic)
│
├── ViewModels/
│   ├── BaseViewModel.cs            INotifyPropertyChanged base class
│   ├── MainViewModel.cs            Transaction list, filters, summary totals, commands
│   └── AddTransactionViewModel.cs  Add and Edit mode in a single form
│
├── Views/
│   ├── MainWindow.xaml             Main window — pure XAML, zero code-behind logic
│   ├── MainWindow.xaml.cs          InitializeComponent() only
│   ├── AddTransactionView.xaml     Add/Edit dialog — pure XAML, zero code-behind logic
│   └── AddTransactionView.xaml.cs  Subscribes to RequestClose event only
│
├── App.xaml.cs                     Composition root — wires all dependencies
│
└── FinanceTracker.Tests/
    └── MainViewModelTests.cs       5 xUnit tests using mock repositories


## Key MVVM Patterns Used

| Pattern | Where |

| `INotifyPropertyChanged` | `BaseViewModel` — inherited by all ViewModels |
| `ObservableCollection<T>` | `Transactions`, `FilteredTransactions`, `Categories` |
| `ICommand` / `RelayCommand` | Add, Edit, Delete, Refresh commands |
| Two-Way data binding | All form inputs in `AddTransactionView.xaml` |
| `CanExecute` | Edit and Delete auto-disable when nothing selected |
| Repository pattern | `ITransactionRepository`, `ICategoryRepository` |
| Interface segregation | Separate interfaces per data concern (SOLID) |
| Dependency injection | Manual wiring in `App.xaml.cs` composition root |
| `RequestClose` event | ViewModel signals View to close — no WPF in ViewModel |
| Unit testing via interface | `MockTransactionRepository` — no real DB in tests |

> **Rule enforced throughout:** Zero logic in `.xaml.cs` files.
> Each View's code-behind contains only `InitializeComponent()`
> (plus `RequestClose` subscription in `AddTransactionView`).



## Database Schema

### Categories Table
| Column | Type | Description |

| `Id` | INTEGER PK | Auto-generated primary key |
| `Name` | TEXT UNIQUE | Display name e.g. "Food" |
| `Type` | INTEGER | 0 = Income, 1 = Expense, 2 = Both |
| `Colour` | TEXT | Hex colour e.g. "#E67E22" |
| `IconName` | TEXT | Icon identifier for future use |
| `IsDefault` | INTEGER | 1 = built-in, 0 = user-created |
| `CreatedAt` | TEXT | ISO date string |

### Transactions Table
| Column | Type | Description |
|---|---|---|
| `Id` | INTEGER PK | Auto-generated primary key |
| `Amount` | REAL | Transaction amount (always positive) |
| `CategoryId` | INTEGER FK | Foreign key → Categories.Id |
| `Date` | TEXT | ISO 8601 datetime string |
| `Note` | TEXT | Optional free-text note |
| `Type` | INTEGER | 0 = Income, 1 = Expense |



## Tech Stack

| Technology | Purpose |
|---|---|
| C# / .NET 6 | Application language and runtime |
| WPF | Desktop UI framework |
| MVVM | Architectural pattern |
| SQLite | Local database (no server required) |
| `Microsoft.Data.Sqlite` | SQLite NuGet driver |
| xUnit | Unit testing framework |



## Getting Started

### Prerequisites
- Visual Studio 2026
- .NET 7.0 SDK or later

### Setup

```bash
git clone https://github.com/KeerthanaJegannathan/FinanceTracker
```

1. Open `FinanceTracker.sln` in Visual Studio 2026
2. Right-click the solution → **Restore NuGet Packages**
3. Set `FinanceTracker` as the startup project
4. Press **F5** to run

`finance.db` is created automatically in the app directory on first launch.

### Run Tests

1. Open **Test Explorer** (View → Test Explorer)
2. Click **Run All**

Or via terminal:
```bash
dotnet test FinanceTracker.Tests
```

---

##  NuGet Packages

### Main Project (FinanceTracker)
| Package | Purpose |
| `Microsoft.Data.Sqlite` | SQLite database driver |

### Test Project (FinanceTracker.Tests)
| Package | Purpose |
| `xunit` | Unit testing framework |
| `xunit.runner.visualstudio` | VS Test Explorer integration |
| `Microsoft.NET.Test.Sdk` | Required for test discovery |

---

## Design Decisions & Rationale

**Why MVVM?**
Clean separation of UI and business logic. ViewModels contain zero WPF references,
making them fully unit-testable without launching a UI or connecting to a database.

**Why interfaces for repositories?**
`ITransactionRepository` and `ICategoryRepository` decouple the ViewModels from SQLite entirely.
In tests, mock repositories are injected instead, no real database needed.
It also means the storage layer can be swapped (e.g. to SQL Server or an API) with zero ViewModel changes.

**Why a single `DatabaseService` implementing both interfaces?**
One SQLite connection manages both tables cleanly. Injecting it as two separate interfaces
follows the Interface Segregation Principle,each ViewModel only sees the methods it needs.

**Why `ObservableCollection<T>` instead of `List<T>`?**
`ObservableCollection` raises change notifications automatically when items are added or removed.
The ListView updates instantly with no manual refresh or UI code required.

**Why `RelayCommand` instead of event handlers?**
All button logic lives in the ViewModel. The View's code-behind stays empty,
which is the gold standard for MVVM discipline and is the first thing reviewers check.

**Why `CanExecute` on Edit and Delete?**
WPF automatically enables and disables the buttons based on whether a row is selected.
No `if` statements or visibility toggles needed in the View.

**Why `RequestClose` event instead of calling `Window.Close()` in the ViewModel?**
ViewModels must never reference WPF classes that would break unit testability.
The ViewModel raises an event,the View's code-behind subscribes and calls `Close()`.
This keeps the ViewModel completely framework-agnostic.

**Why `DisplayMemberPath="Name"` on the Category ComboBox?**
The ComboBox binds to `ObservableCollection<Category>` (full objects, not strings)
so `SelectedCategory.Id` is available as the foreign key when saving.
`DisplayMemberPath` tells WPF to show only the `Name` property as the label.

**Why manual dependency injection instead of a DI container?**
For a focused portfolio project, manual wiring in `App.xaml.cs` is clear and easy to explain.
In a production app, `Microsoft.Extensions.DependencyInjection` would replace this.

## Screenshot

<img width="700" height="500" alt="AddTransaction" src="https://github.com/user-attachments/assets/34a89d1a-0432-4e19-a984-8b5725b23d55" />

<img width="700" height="500" alt="Appilcation" src="https://github.com/user-attachments/assets/a177c34f-9401-4816-aadc-bdab4411ed5d" />

<img width="700" height="500" alt="IsOverBudget" src="https://github.com/user-attachments/assets/10291ad1-4ffd-4177-9016-fedb58688dbd" />

## About

Built as a portfolio project to demonstrate WPF and MVVM expertise during a career
return to software development after a planned career break.

Part of an ongoing upskilling journey expanding into ASP.NET Core Web API
and modern full-stack development.
