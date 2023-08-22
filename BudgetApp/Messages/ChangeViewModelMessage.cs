using BudgetApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.Messages;

public class ChangeViewModelMessage
{
    public ViewModelBase? ViewModel { get; set; }
}
