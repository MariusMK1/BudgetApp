
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace BudgetApp.ViewModels;

public class ViewModelBase : ObservableRecipient
{
    public ViewModelBase()
    {
    }
    public ViewModelBase(IMessenger messenger) : base(messenger)
    {
    }
}