using BudgetApp.Helpers;
using BudgetApp.Messages;
using BudgetApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.ViewModels;

public partial class IncomeViewModel : ViewModelBase
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private double? _amount;
    [ObservableProperty] private string? _tag;
    public string Id { get; set; } = string.Empty;
    public IncomeViewModel(IMessenger messenger) : base(messenger)
    {
        Name = string.Empty;
        Amount = 10;
    }
    public void Initialize(IncomeItem item)
    {
        Name = item.Name;
        Amount = item.Amount;
        Tag = item.Tag;
        Id = item.Id;
    }
    [RelayCommand]
    async Task Save(object parameter)
    {
        var values = parameter as IList;
        if (values?.Count >= 2)
        {
            var name = values[0] as string;
            if (string.IsNullOrEmpty(name))
                return;
            var number = values[1] as IConvertible;
            if (number == null)
                return;
            Name = char.ToUpper(name[0]) + name.Substring(1);
            Amount = number.ToDouble(null);
            var tag = values[2] as string;
            if (string.IsNullOrEmpty(tag))
                Tag = string.Empty;
            else
                Tag = char.ToUpper(tag[0]) + tag.Substring(1);

            var newIncomeItem = new IncomeItem()
            {
                Name = Name,
                Amount = Amount,
                Tag = Tag,
                Id = Id
            };
            if (string.IsNullOrEmpty(Id))
            {
                await DatabaseHelper.Insert(newIncomeItem);
                Id = newIncomeItem.Id;
            }
            else
                await DatabaseHelper.Update(newIncomeItem);
        }
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = null });
    }
    [RelayCommand]
    void Cancel()
    {
        Messenger.Send(new ChangeViewModelMessage() { ViewModel = null });
    }
}
