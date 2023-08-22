using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using BudgetApp.ViewModels;

namespace BudgetApp.Views;

public partial class ParentView : UserControl
{
    public ParentView()
    {
        InitializeComponent();
    }
    private void SplitView_PaneClosed(object sender, RoutedEventArgs e)
    {
        if (DataContext is ParentViewModel viewModel)
        {
            viewModel.OnPaneClosed();
        }
    }
}