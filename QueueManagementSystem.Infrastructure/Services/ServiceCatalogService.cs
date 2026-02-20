using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.Application.DTOs.Services;
using QueueManagementSystem.Application.Interfaces;
using QueueManagementSystem.Domain.Entities;
using QueueManagementSystem.Infrastructure.Persistence;

namespace QueueManagementSystem.Infrastructure.Services
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly QueueDbContext _context;

        public ServiceCatalogService(QueueDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<ServiceDto>> GetServicesAsync(int? branchId)
        {
            IQueryable<Service> query = _context.Services.Include(service => service.Branch);

            if (branchId.HasValue)
            {
                query = query.Where(service => service.BranchId == branchId);
            }

            List<ServiceDto> services = await query
                .OrderBy(service => service.ServiceName)
                .Select(service => new ServiceDto
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    AvgServiceTime = service.AvgServiceTime,
                    IsOpen = service.IsOpen,
                    BranchId = service.BranchId,
                    BranchName = service.Branch != null ? service.Branch.BranchName : null
                })
                .ToListAsync();

            return services;
        }

        public async Task<ServiceDto> GetServiceAsync(int serviceId)
        {
            Service service = await _context.Services
                .Include(current => current.Branch)
                .FirstOrDefaultAsync(current => current.Id == serviceId);

            if (service == null)
            {
                return null;
            }

            return new ServiceDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                AvgServiceTime = service.AvgServiceTime,
                IsOpen = service.IsOpen,
                BranchId = service.BranchId,
                BranchName = service.Branch?.BranchName
            };
        }

        public async Task<ServiceDto> CreateServiceAsync(ServiceCreateDto request)
        {
            Service service = new Service
            {
                ServiceName = request.ServiceName,
                AvgServiceTime = request.AvgServiceTime,
                BranchId = request.BranchId,
                IsOpen = true
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return await GetServiceAsync(service.Id);
        }

        public async Task<ServiceDto> UpdateServiceAsync(int serviceId, ServiceUpdateDto request)
        {
            Service service = await _context.Services.FirstOrDefaultAsync(current => current.Id == serviceId);

            if (service == null)
            {
                return null;
            }

            service.ServiceName = request.ServiceName;
            service.AvgServiceTime = request.AvgServiceTime;
            service.IsOpen = request.IsOpen;
            service.BranchId = request.BranchId;

            await _context.SaveChangesAsync();

            return await GetServiceAsync(service.Id);
        }

        public async Task SetServiceStatusAsync(int serviceId, bool isOpen)
        {
            Service service = await _context.Services.FirstOrDefaultAsync(current => current.Id == serviceId);

            if (service == null)
            {
                return;
            }

            service.IsOpen = isOpen;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            Service service = await _context.Services.FirstOrDefaultAsync(current => current.Id == serviceId);

            if (service == null)
            {
                return;
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
    }
}
