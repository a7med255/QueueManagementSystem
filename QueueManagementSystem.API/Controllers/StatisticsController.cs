using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagementSystem.Application.DTOs.Statistics;
using QueueManagementSystem.Application.Interfaces;

namespace QueueManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("services/{serviceId:int}/branches/{branchId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceStatisticsDto>> GetServiceStatistics(int serviceId, int branchId)
        {
            ServiceStatisticsDto stats = await _statisticsService.GetServiceStatisticsAsync(serviceId, branchId);
            if (stats == null)
            {
                return NotFound();
            }

            return Ok(stats);
        }
    }
}
