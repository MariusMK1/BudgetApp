using Avalonia;
using Avalonia.Controls;
using BudgetApp.Helpers;
using BudgetApp.Messages;
using BudgetApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace BudgetApp.ViewModels;

public partial class ExpensesViewModel : MenuViewModel
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private ExpenseViewModel? _selectedExpenseViewModel;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _addedExpense;
    public ObservableCollection<ExpenseViewModel> Expenses { get; set; }
    public ExpensesViewModel(IMessenger messenger) : base(messenger)
    {
        Name = "This is Expenses View";
        Expenses = new();
    }
    public async Task Initialize()
    {
        var expenseItems = (await DatabaseHelper.Read<ExpenseItem>()).Where(x => x.UserId == App.UserId);
        if (expenseItems is null)
            return;
        foreach (var expenseItem in expenseItems)
        {
            var vm = new ExpenseViewModel(Messenger);
            vm.Initialize(expenseItem);
            Expenses.Add(vm);
        }
    }
    partial void OnSelectedExpenseViewModelChanged(ExpenseViewModel? oldValue, ExpenseViewModel? newValue)
    {
        IsSelected = newValue is not null ? true : false;
    }
    partial void OnAddedExpenseChanged(bool value)
    {
        if (value)
        {
            var timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                AddedExpense = false;
            };
            timer.Start();
        }
    }
    [RelayCommand]
    async Task AddExpense()
    {
        if (SelectedExpenseViewModel is null)
            return;
        var expense = new Expense()
        {
            UserId = App.UserId,
            Amount = SelectedExpenseViewModel.Amount,
            Tag = SelectedExpenseViewModel.Tag,
            Name = SelectedExpenseViewModel.Name,
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
        await DatabaseHelper.Insert(expense);
        AddedExpense = true;
        Messenger.Send(new AddedIncomeOrExpense());
    }
    [RelayCommand]
    void ModifyExpense()
    {
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = SelectedExpenseViewModel });
    }
    [RelayCommand]
    void CreateNewExpense()
    {
        var newExpense = new ExpenseViewModel(Messenger) { Name = "New Expense" };
        Expenses.Add(newExpense);
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = newExpense });
    }
    [RelayCommand]
    void CopyExpense()
    {
        var newExpense = new ExpenseViewModel(Messenger) { Name = SelectedExpenseViewModel?.Name + " - Copy", Amount = SelectedExpenseViewModel?.Amount, Tag = SelectedExpenseViewModel?.Tag };
        Expenses.Add(newExpense);
    }
    [RelayCommand]
    async Task DeleteExpense()
    {
        if (SelectedExpenseViewModel is null)
            return;
        var expenseItem = new ExpenseItem()
        {
            Id = SelectedExpenseViewModel.Id
        };
        if (!string.IsNullOrEmpty(expenseItem.Id))
            await DatabaseHelper.Delete(expenseItem);
        Expenses.Remove(SelectedExpenseViewModel);
    }
}
