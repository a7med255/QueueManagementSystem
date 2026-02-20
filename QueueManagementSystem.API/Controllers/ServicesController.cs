using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagementSystem.Application.DTOs.Services;
using QueueManagementSystem.Application.Interfaces;

namespace QueueManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceCatalogService _serviceCatalogService;

        public ServicesController(IServiceCatalogService serviceCatalogService)
        {
            _serviceCatalogService = serviceCatalogService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<ServiceDto>>> GetServices([FromQuery] int? branchId)
        {
            IReadOnlyCollection<ServiceDto> services = await _serviceCatalogService.GetServicesAsync(branchId);
            return Ok(services);
        }

        [HttpGet("{serviceId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceDto>> GetService(int serviceId)
        {
            ServiceDto service = await _serviceCatalogService.GetServiceAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceDto>> CreateService(ServiceCreateDto request)
        {
            ServiceDto service = await _serviceCatalogService.CreateServiceAsync(request);
            return CreatedAtAction(nameof(GetService), new { serviceId = service.Id }, service);
        }

        [HttpPut("{serviceId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceDto>> UpdateService(int serviceId, ServiceUpdateDto request)
        {
            ServiceDto service = await _serviceCatalogService.UpdateServiceAsync(serviceId, request);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        [HttpPatch("{serviceId:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int serviceId, [FromQuery] bool isOpen)
        {
            await _serviceCatalogService.SetServiceStatusAsync(serviceId, isOpen);
            return NoContent();
        }

        [HttpDelete("{serviceId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            await _serviceCatalogService.DeleteServiceAsync(serviceId);
            return NoContent();
        }
    }
}
