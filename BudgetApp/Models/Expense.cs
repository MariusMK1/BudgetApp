﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.Models;

public class Expense: IHasId
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double? Amount { get; set; }
    public string? Tag { get; set; }
    public string UserId { get; set; } = App.UserId;
    public DateOnly Date { get; set; }
}
