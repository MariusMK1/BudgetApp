using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BudgetApp.Views;

public partial class BudgetView : UserControl
{
    public BudgetView()
    {
        InitializeComponent();
    }
    private void TagListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var VM = (DataContext as ViewModels.BudgetViewModel);
        if (VM is null)
            return;
        if (e.AddedItems.Count > 0)
            VM.SelectedTags.Add(e.AddedItems[0].ToString());
        if (e.RemovedItems.Count > 0)
            VM.SelectedTags.Remove(e.RemovedItems[0].ToString());
    }
}