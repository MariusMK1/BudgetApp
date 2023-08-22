using Avalonia.Input;
using Avalonia.Interactivity;
using BudgetApp.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{ 
    [ObservableProperty] private ViewModelBase _childViewModel;

    public MainViewModel(IMessenger messenger): base(messenger)
    {
        ChildViewModel = new LoginViewModel(Messenger);
        Messenger.Register<LogedInMessage>(this, UserLogedIn);
    }
    private async void UserLogedIn(object recipient, LogedInMessage message)
    {
        if (message.LogedIn)
        {
            var parentViewModel = new ParentViewModel(Messenger);
            await parentViewModel.Initialize();
            ChildViewModel = parentViewModel;
        }
        else
        {
            ChildViewModel = new LoginViewModel(Messenger);
        }
    }
}