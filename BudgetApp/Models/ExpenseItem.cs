namespace BudgetApp.Models;

public class ExpenseItem : IHasId
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double? Amount { get; set; }
    public string? Tag { get; set; }
    public string UserId { get; set; } = App.UserId;
}