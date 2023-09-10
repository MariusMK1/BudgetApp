using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BudgetApp.Helpers;
using BudgetApp.Models;
using System.Linq;
using System;
using System.Globalization;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using Avalonia;
using LiveChartsCore.Kernel;
using CommunityToolkit.Mvvm.Messaging;

namespace BudgetApp.ViewModels;

public partial class BudgetViewModel : MenuViewModel
{
    [ObservableProperty] IEnumerable<ISeries>? _series = new List<ISeries>();
    public List<Expense> Expenses = new();
    public List<Income> Incomes = new();

    [ObservableProperty] double _total;
    [ObservableProperty] double _totalExpense;
    [ObservableProperty] double _totalIncome;
    [ObservableProperty] double _profitLoss;
    [ObservableProperty] private string? _selectedMonth;

    [ObservableProperty]
    private ISeries[]? _seriesBars;
    [ObservableProperty]
    private Axis[] _xAxes = { new Axis { SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) } };
    [ObservableProperty]
    private Axis[] _yAxes = { new Axis { IsVisible = false } };
    [ObservableProperty] private bool _useRacingBars;

    public ObservableCollection<string> Months { get; set; }
    public ObservableCollection<string> Tags { get; set; }
    public ObservableCollection<string> SelectedTags { get; set; }
    private Random random = new();
    public BudgetViewModel(IMessenger messenger): base(messenger)
    {
        Months = new();
        Tags = new();
        SelectedTags = new();
        SelectedTags.CollectionChanged += SelectedTags_CollectionChanged;
    }
    public async Task Initialize()
    {
        var expenses = (await DatabaseHelper.Read<Expense>()).Where(x => x.UserId == App.UserId).ToList();
        if (expenses is not null)
            Expenses.AddRange(expenses);
        var incomes = (await DatabaseHelper.Read<Income>()).Where(x => x.UserId == App.UserId).ToList();
        if (incomes is not null)
            Incomes.AddRange(incomes);
        if (!Incomes.Any() && !Expenses.Any())
            return;
        CalculateUniqueMonths();
        GetAllTags();
        SelectedMonth = Months[^2];
        NewUpSeries();
    }

    private void NewUpSeries()
    {
        if (UseRacingBars)
            BuildRacingBars();
        else
            BuildGauge();
    }
    partial void OnUseRacingBarsChanged(bool value)
    {
        NewUpSeries();
    }
    private void BuildRacingBars()
    {
        var initialData = new List<(string, double)>
        {
            ("Expense", TotalExpense),
            ("Income", TotalIncome)
        };
        foreach(var tag in SelectedTags)
        {
            var incomesByTag = Incomes.Where(x => x.Tag == tag);
            var expensesByTag = Expenses.Where(x => x.Tag == tag);
            double income = 0;
            double expense = 0;
            if (SelectedMonth.Equals("All"))
            {
                income = incomesByTag.Sum(x => x.Amount ?? 0);
                expense = expensesByTag.Sum(x => x.Amount ?? 0);
            }
            else
            {
                income = incomesByTag.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == SelectedMonth).ToList().Sum(x => x.Amount ?? 0);
                expense = expensesByTag.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == SelectedMonth).ToList().Sum(x => x.Amount ?? 0);
            }
            if (expense != 0)
                initialData.Add(($"{tag} Expense", expense));
            if (income != 0)
                initialData.Add(($"{tag} Income", income));
        }
        SeriesBars = initialData
            .Select(x => new RowSeries<ObservableValue>
            {
                Values = new[] { new ObservableValue(x.Item2) },
                Name = x.Item1,
                Stroke = null,
                MaxBarWidth = 25,
                DataLabelsPaint = new SolidColorPaint(new SKColor(245, 245, 245)),
                DataLabelsTranslate = new LvcPoint(1.2, 0.06),
                DataLabelsPosition = DataLabelsPosition.Start,
                DataLabelsFormatter = point => $"{point.Context.Series.Name} {point.PrimaryValue}€"
            })
            .OrderByDescending(x => ((ObservableValue[])x.Values!)[0].Value)
            .ToArray();
    }

    private void BuildGauge()
    {
        var gougeBuilder = new GaugeBuilder()
          .WithOffsetRadius(5)
          .WithLabelsPosition(PolarLabelsPosition.Start)
          .WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name} ")
          .AddValue(TotalExpense, "€ Expense", GetRandomColor(), SKColors.White)
          .AddValue(TotalIncome, "€ Income", GetRandomColor(), SKColors.White);

        foreach (var tag in SelectedTags)
        {
            var incomesByTag = Incomes.Where(x => x.Tag == tag);
            var expensesByTag = Expenses.Where(x => x.Tag == tag);
            double income = 0;
            double expense = 0;
            if (SelectedMonth.Equals("All"))
            {
                income = incomesByTag.Sum(x => x.Amount ?? 0);
                expense = expensesByTag.Sum(x => x.Amount ?? 0);
            }
            else
            {
                income = incomesByTag.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == SelectedMonth).ToList().Sum(x => x.Amount ?? 0);
                expense = expensesByTag.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == SelectedMonth).ToList().Sum(x => x.Amount ?? 0);
            }
            if (expense != 0)
                gougeBuilder.AddValue(expense, $"€ {tag} Expense", GetRandomColor(), SKColors.White);
            if (income != 0)
                gougeBuilder.AddValue(income, $"€ {tag} Income", GetRandomColor(), SKColors.White);
        }
        Series = gougeBuilder.BuildSeries();
    }

    private SKColor GetRandomColor()
    {
        byte[] colorBytes = new byte[4];
        random.NextBytes(colorBytes);
        return new SKColor(colorBytes[0], colorBytes[1], colorBytes[2], colorBytes[3]);
    }
    private void CalculateUniqueMonths()
    {
        var uniqueIncomeMonths = Incomes.Select(income => new DateTime(income.Date.Year, income.Date.Month, 1)).Distinct().ToList();
        var uniqueExpenseMonths = Expenses.Select(expense => new DateTime(expense.Date.Year, expense.Date.Month, 1)).Distinct().ToList();
        var uniqueMonths = uniqueIncomeMonths.Union(uniqueExpenseMonths).Distinct().ToList();
        var uniqueMonthStrings = uniqueMonths.Select(month => month.ToString("MMMM yyyy", CultureInfo.InvariantCulture)).ToList();
        uniqueMonthStrings.Reverse();

        foreach (var month in uniqueMonthStrings)
            Months.Add(month);
        Months.Add("All");
    }
    private void GetAllTags()
    {
        var uniqueIncomeTags = Incomes.Select(income => income.Tag).Distinct().ToList();
        var uniqueExpenseTags = Expenses.Select(expense => expense.Tag).Distinct().ToList();
        var uniqueTags = uniqueIncomeTags.Union(uniqueExpenseTags).Distinct().ToList();
        foreach (var tag in uniqueTags)
            Tags.Add(tag);
    }

    private void CalculateTotals(List<Income> incomes, List<Expense> expenses)
    {
        TotalExpense = expenses.Sum(x => x.Amount ?? 0);
        TotalIncome = incomes.Sum(x => x.Amount ?? 0);
        ProfitLoss = TotalIncome - TotalExpense;
        Total = Math.Max(TotalIncome, TotalExpense);
    }

    partial void OnSelectedMonthChanged(string? value)
    {
        if (value is null)
            return;
        if (value.Equals("All"))
        {
            CalculateTotals(Incomes, Expenses);
        }
        else
        {
            var incomesByMonth = Incomes.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == value).ToList();
            var expensesByMonth = Expenses.Where(x => x.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture) == value).ToList();
            CalculateTotals(incomesByMonth, expensesByMonth);
        }
        NewUpSeries();
    }
    private void SelectedTags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (!UseRacingBars)
        {
            var tempMonth = SelectedMonth;
            if (string.IsNullOrEmpty(SelectedMonth))
                throw new Exception("SelectedMonth is null or empty");
            if (SelectedMonth.Equals("All"))
                SelectedMonth = Months[0];
            else
                SelectedMonth = Months.Last();
            SelectedMonth = tempMonth;
        }
        NewUpSeries();
    }
}