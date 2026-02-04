using System.Threading.Tasks;
using QueueManagementSystem.Application.DTOs.Statistics;

namespace QueueManagementSystem.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<ServiceStatisticsDto> GetServiceStatisticsAsync(int serviceId, int branchId);
    }
}
