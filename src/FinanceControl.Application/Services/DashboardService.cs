using FinanceControl.Application.DTOs.Dashboard;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Enums;
using FinanceControl.Domain.Interfaces;

namespace FinanceControl.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        Guid userId, int? month = null, int? year = null)
    {
        var currentDate = DateTime.UtcNow;
        var targetMonth = month ?? currentDate.Month;
        var targetYear = year ?? currentDate.Year;

        // Resumo do mês solicitado
        var monthTransactions = await _unitOfWork.Transactions
            .GetByUserIdAndMonthAsync(userId, targetYear, targetMonth);

        var totalIncome = monthTransactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = monthTransactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        // Evolução dos últimos 6 meses
        var monthlyEvolution = new List<MonthlyEvolutionDto>();

        for (int i = 5; i >= 0; i--)
        {
            var date = new DateTime(targetYear, targetMonth, 1)
                .AddMonths(-i);

            var monthData = await _unitOfWork.Transactions
                .GetByUserIdAndMonthAsync(userId, date.Year, date.Month);

            var income = monthData
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var expense = monthData
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            monthlyEvolution.Add(new MonthlyEvolutionDto
            {
                Month = date.Month,
                Year = date.Year,
                MonthName = date.ToString("MMM/yyyy"),
                TotalIncome = income,
                TotalExpense = expense,
                Balance = income - expense
            });
        }

        // Distribuição por categoria (apenas despesas)
        var expenseTransactions = monthTransactions
            .Where(t => t.Type == TransactionType.Expense)
            .ToList();

        var categorySummary = expenseTransactions
            .GroupBy(t => new
            {
                t.CategoryId,
                CategoryName = t.Category?.Name ?? "Sem categoria",
                CategoryColor = t.Category?.Color ?? "#CCCCCC"
            })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                CategoryColor = g.Key.CategoryColor,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count(),
                Percentage = totalExpense > 0
                    ? Math.Round(g.Sum(t => t.Amount) / totalExpense * 100, 2)
                    : 0
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return new DashboardSummaryDto
        {
            Month = targetMonth,
            Year = targetYear,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = totalIncome - totalExpense,
            MonthlyEvolution = monthlyEvolution,
            CategorySummary = categorySummary
        };
    }
}