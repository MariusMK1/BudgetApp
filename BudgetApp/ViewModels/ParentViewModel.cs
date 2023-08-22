using Avalonia.Input;
using BudgetApp.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class ParentViewModel : ViewModelBase
{
    [ObservableProperty] private bool _menuOpen;
    [ObservableProperty] private ViewModelBase _contentViewModel;
    private IncomesViewModel? _incomesViewModel { get; set; }
    private ExpensesViewModel? _expensesViewModel { get; set; }
    private BudgetViewModel? _budgetViewModel { get; set; }
    private ViewModelBase? _previousViewModel { get; set; }
    public event EventHandler? PaneClosed;
    public ParentViewModel(IMessenger messenger) : base(messenger)
    {
        Messenger.Register<ChangeViewModelMessage>(this, OnViewCreated);
        Messenger.Register<AddedIncomeOrExpense>(this, NullBudegetViewModel);
        ContentViewModel = new();
    }

    public async Task Initialize()
    {
        _expensesViewModel = new ExpensesViewModel(Messenger);
        await _expensesViewModel.Initialize();
        ContentViewModel = _expensesViewModel;
    }
    public void OnPaneClosed()
    {
        MenuOpen = false;
    }
    private void OnViewCreated(object recipient, ChangeViewModelMessage message)
    {
        if (message.ViewModel is not null)
        {
            _previousViewModel = ContentViewModel;
            ContentViewModel = message.ViewModel;
        }
        else
        {
            if (_previousViewModel is null)
                return;
            ContentViewModel = _previousViewModel;
        }
    }
    private void NullBudegetViewModel(object recipient, AddedIncomeOrExpense message)
    {
        _budgetViewModel = null;
    }
    [RelayCommand]
    private void MenuClick()
    {
        MenuOpen = true;
    }
    [RelayCommand]
    private void CloseMenu(PointerPressedEventArgs args)
    {
        MenuOpen = false;
    }
    [RelayCommand]
    private async Task Budget(PointerPressedEventArgs args)
    {
        if (_budgetViewModel is null)
        {
            _budgetViewModel = new BudgetViewModel();
            await _budgetViewModel.Initialize();
        }
        ContentViewModel = _budgetViewModel;
        MenuOpen = false;
    }
    [RelayCommand]
    private void Expenses(PointerPressedEventArgs args)
    {
        if (_expensesViewModel is null)
            _expensesViewModel = new ExpensesViewModel(Messenger);
        ContentViewModel = _expensesViewModel;
        MenuOpen = false;
    }
    [RelayCommand]
    private async Task Income(PointerPressedEventArgs args)
    {
        if (_incomesViewModel is null)
        {
            _incomesViewModel = new IncomesViewModel(Messenger);
            await _incomesViewModel.Initialize();
        }
        ContentViewModel = _incomesViewModel;
        MenuOpen = false;
    }
    [RelayCommand]
    private void Logout(PointerPressedEventArgs args)
    {
        Messenger.Send(new LogedInMessage { LogedIn = false });
    }
}
