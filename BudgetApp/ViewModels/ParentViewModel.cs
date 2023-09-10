using Avalonia.Input;
using BudgetApp.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class ParentViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _contentViewModel;
    private IncomesViewModel? _incomesViewModel { get; set; }
    private ExpensesViewModel? _expensesViewModel { get; set; }
    private BudgetViewModel? _budgetViewModel { get; set; }
    private ViewModelBase? _previousViewModel { get; set; }
    public event EventHandler? PaneClosed;
    public ParentViewModel(IMessenger messenger) : base(messenger)
    {
        ContentViewModel = new();
    }

    public async Task Initialize()
    {
        Messenger.Register<ChangeViewModelMessage>(this, async (recipient, message) => await OnViewCreated(recipient, message));
        Messenger.Register<AddedIncomeOrExpense>(this, NullBudegetViewModel);
        _expensesViewModel = new ExpensesViewModel(Messenger);
        await _expensesViewModel.Initialize();
        ContentViewModel = _expensesViewModel;
    }
    private async Task OnViewCreated(object recipient, ChangeViewModelMessage message)
    {
        if (message.ViewModel is not null)
        {
            if (message.ViewModel is BudgetViewModel)
                await Budget();
            else if (message.ViewModel is ExpensesViewModel)
                Expenses();
            else if (message.ViewModel is IncomesViewModel)
                await Income();
            else if (message.ViewModel is ExpenseViewModel || message.ViewModel is IncomeViewModel)
            {
                _previousViewModel = ContentViewModel;
                ContentViewModel = message.ViewModel;
            }
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
    private async Task Budget()
    {
        if (_budgetViewModel is null)
        {
            _budgetViewModel = new BudgetViewModel(Messenger);
            await _budgetViewModel.Initialize();
        }
        _previousViewModel = ContentViewModel;
        ContentViewModel = _budgetViewModel;
    }
    private void Expenses()
    {
        if (_expensesViewModel is null)
            _expensesViewModel = new ExpensesViewModel(Messenger);
        _previousViewModel = ContentViewModel;
        ContentViewModel = _expensesViewModel;
    }
    private async Task Income()
    {
        if (_incomesViewModel is null)
        {
            _incomesViewModel = new IncomesViewModel(Messenger);
            await _incomesViewModel.Initialize();
        }
        _previousViewModel = ContentViewModel;
        ContentViewModel = _incomesViewModel;
    }
    [RelayCommand]
    private void Logout(PointerPressedEventArgs args)
    {
        Messenger.Send(new LogedInMessage { LogedIn = false });
    }
}
