using FinanceControl.Application.DTOs.Dashboard;

namespace FinanceControl.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(Guid userId, int? month = null, int? year = null);
}