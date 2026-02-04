using System.Collections.Generic;
using System.Threading.Tasks;
using QueueManagementSystem.Application.DTOs.Services;

namespace QueueManagementSystem.Application.Interfaces
{
    public interface IServiceCatalogService
    {
        Task<IReadOnlyCollection<ServiceDto>> GetServicesAsync(int? branchId);
        Task<ServiceDto> GetServiceAsync(int serviceId);
        Task<ServiceDto> CreateServiceAsync(ServiceCreateDto request);
        Task<ServiceDto> UpdateServiceAsync(int serviceId, ServiceUpdateDto request);
        Task SetServiceStatusAsync(int serviceId, bool isOpen);
        Task DeleteServiceAsync(int serviceId);
    }
}
