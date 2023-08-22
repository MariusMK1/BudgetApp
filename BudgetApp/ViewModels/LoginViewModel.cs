using Avalonia;
using BudgetApp.Helpers;
using BudgetApp.Messages;
using BudgetApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;
    [ObservableProperty] private string _confirmPassword;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _showSignUpView;
    [ObservableProperty] private bool _canSignUp;
    [ObservableProperty] private bool _canLogin;
    [ObservableProperty] private bool _showError;
    [ObservableProperty] private bool _rememberMe;
    public LoginViewModel(IMessenger messenger) : base(messenger)
    {
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
    }

    partial void OnConfirmPasswordChanged(string value)
    {
        SetUpCanSignUp();
    }
    partial void OnPasswordChanged(string value)
    {
        if (ShowSignUpView)
            SetUpCanSignUp();
        else
            SetUpCanLogin();
    }
    partial void OnEmailChanging(string value)
    {
        if (ShowSignUpView)
            SetUpCanSignUp();
        else
            SetUpCanLogin();
    }

    private void SetUpCanLogin()
    {
        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email) && Email.Contains("@"))
            CanLogin = true;
        else
            CanLogin = false;
    }

    private void SetUpCanSignUp()
    {
        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && Password == ConfirmPassword && Password.Length >= 6 && !string.IsNullOrEmpty(Email) && Email.Contains("@"))
            CanSignUp = true;
        else
            CanSignUp = false;
    }

    [RelayCommand]
    private async Task Login()
    {
        var user = new User
        {
            Email = Email.Trim(),
            Password = Password
        };
        var loginStatus = await FirebaseAuthHelper.Login(user);
        if (loginStatus.Success)
            Messenger.Send(new LogedInMessage { LogedIn = true});
        else
        {
            ShowError = true;
            ErrorMessage = loginStatus.Message;
        }
    }
    [RelayCommand]
    private void SignUp()
    {
        ShowError = false;
        ShowSignUpView = true;
    }
    [RelayCommand]
    private void Cancel()
    {
        ShowError = false;
        ShowSignUpView = false;
    }
    [RelayCommand]
    private async Task SignUpNew()
    {
        var user = new User
        {
            Email = Email.Trim(),
            Password = Password,
            ConfirmPassword = ConfirmPassword
        };
        var loginStatus = await FirebaseAuthHelper.Register(user);
        if (loginStatus.Success)
        {
            ShowSignUpView = false;
            ShowError = false;
        }
        else
        { 
            ShowError = true;
            ErrorMessage = loginStatus.Message;
        }
    }
}
