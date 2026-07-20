using Elo.Application.DTOs.Dashboard;

namespace Elo.Application.Services;

public interface IDashboardService
{
    Task<DashboardResumoDto> ObterResumoAsync(CancellationToken ct = default);
}
