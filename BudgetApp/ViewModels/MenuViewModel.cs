using Avalonia.Input;
using BudgetApp.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class MenuViewModel : ViewModelBase
{
    public MenuViewModel(IMessenger messenger): base(messenger)
    {
    }
    [RelayCommand]
    private void Budget(PointerPressedEventArgs args)
    {
        Messenger.Send(new ChangeViewModelMessage { ViewModel = new BudgetViewModel(Messenger) });
    }
    [RelayCommand]
    private void Expenses(PointerPressedEventArgs args)
    {
        Messenger.Send(new ChangeViewModelMessage { ViewModel = new ExpensesViewModel(Messenger) });
    }
    [RelayCommand]
    private void Income(PointerPressedEventArgs args)
    {
        Messenger.Send(new ChangeViewModelMessage { ViewModel = new IncomesViewModel(Messenger) });
    }
}
