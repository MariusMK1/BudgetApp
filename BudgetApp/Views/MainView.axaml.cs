using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using BudgetApp.ViewModels;

namespace BudgetApp.Views;

public partial class MainView : UserControl
{
    private double _widnowHeight { get; set; }
    public MainView()
    {
        InitializeComponent();
    }
}