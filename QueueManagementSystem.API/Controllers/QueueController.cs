using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagementSystem.Application.DTOs.Tickets;
using QueueManagementSystem.Application.Interfaces;
using System.Security.Claims;

namespace QueueManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost("tickets")]
        [Authorize]
        public async Task<ActionResult<TicketDto>> CreateTicket( CreateTicketRequestDto request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            TicketDto ticket = await _queueService.CreateTicketAsync(userId, request);

            if (ticket == null)
            {
                return BadRequest("Service is closed or unavailable.");
            }

            return Ok(ticket);
        }

        [HttpGet("services/{serviceId:int}/branches/{branchId:int}")]
        [Authorize]
        public async Task<ActionResult<QueueStatusDto>> GetQueue(int serviceId, int branchId)
        {
            QueueStatusDto queue = await _queueService.GetQueueAsync(serviceId, branchId);
            if (queue == null)
            {
                return NotFound();
            }

            return Ok(queue);
        }

        [HttpPost("call-next")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TicketDto>> CallNext(TicketActionRequestDto request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketDto ticket = await _queueService.CallNextAsync(request, userId);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPost("tickets/{ticketId:int}/skip")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TicketDto>> SkipTicket(int ticketId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketDto ticket = await _queueService.SkipAsync(ticketId, userId);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPost("tickets/{ticketId:int}/cancel")]
        [Authorize]
        public async Task<ActionResult<TicketDto>> CancelTicket(int ticketId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketDto ticket = await _queueService.CancelAsync(ticketId, userId);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
    }
}
