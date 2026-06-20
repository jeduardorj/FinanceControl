namespace FinanceControl.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public IEnumerable<MonthlyEvolutionDto> MonthlyEvolution { get; set; }
        = new List<MonthlyEvolutionDto>();
    public IEnumerable<CategorySummaryDto> CategorySummary { get; set; }
        = new List<CategorySummaryDto>();
}