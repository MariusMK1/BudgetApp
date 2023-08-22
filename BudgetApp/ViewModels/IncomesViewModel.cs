using BudgetApp.Helpers;
using BudgetApp.Messages;
using BudgetApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace BudgetApp.ViewModels;

public partial class IncomesViewModel: ViewModelBase
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private IncomeViewModel? _selectedIncomeViewModel;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _addedIncome;
    public ObservableCollection<IncomeViewModel> Incomes { get; set; }
    public IncomesViewModel(IMessenger messenger) : base(messenger)
    {
        Name = "This is Incomes View";
        Incomes = new ();
    }
    public async Task Initialize()
    {
        var incomeItems = (await DatabaseHelper.Read<IncomeItem>()).Where(x => x.UserId == App.UserId);
        if (incomeItems is null)
            return;
        foreach (var incomeItem in incomeItems)
        {
            var vm = new IncomeViewModel(Messenger);
            vm.Initialize(incomeItem);
            Incomes.Add(vm);
        }
    }
    partial void OnSelectedIncomeViewModelChanged(IncomeViewModel? oldValue, IncomeViewModel? newValue)
    {
        IsSelected = newValue is not null ? true : false;
    }
    partial void OnAddedIncomeChanged(bool value)
    {
        if (value)
        {
            var timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                AddedIncome = false;
            };
            timer.Start();
        }
    }
    [RelayCommand]
    async Task AddIncome()
    {
        if (SelectedIncomeViewModel is null)
            return;
        var income = new Income()
        {
            UserId = App.UserId,
            Amount = SelectedIncomeViewModel.Amount,
            Tag = SelectedIncomeViewModel.Tag,
            Name = SelectedIncomeViewModel.Name,
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
        await DatabaseHelper.Insert(income);
        AddedIncome = true;
        Messenger.Send(new AddedIncomeOrExpense());
    }
    [RelayCommand]
    void ModifyIncome()
    {
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = SelectedIncomeViewModel });
    }
    [RelayCommand]
    void CreateNewIncome()
    {
        var newIncome = new IncomeViewModel(Messenger) { Name = "New Income" };
        Incomes.Add(newIncome);
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = newIncome });
    }
    [RelayCommand]
    void CopyIncome()
    {
        var newExpense = new IncomeViewModel(Messenger) { Name = SelectedIncomeViewModel?.Name + " - Copy", Amount = SelectedIncomeViewModel?.Amount, Tag = SelectedIncomeViewModel?.Tag };
        Incomes.Add(newExpense);
    }
    [RelayCommand]
    async Task DeleteIncome()
    {
        if (SelectedIncomeViewModel is null)
            return;
        var incomeItem = new IncomeItem()
        {
            Id = SelectedIncomeViewModel.Id
        };
        if(!string.IsNullOrEmpty(incomeItem.Id))
            await DatabaseHelper.Delete(incomeItem);
        Incomes.Remove(SelectedIncomeViewModel);
    }
}
