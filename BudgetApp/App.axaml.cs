using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BudgetApp.ViewModels;
using BudgetApp.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace BudgetApp
{
    public partial class App : Application
    {
        internal static string UserId { get; set; } = string.Empty;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(new WeakReferenceMessenger())
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = new MainViewModel(new WeakReferenceMessenger())
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}