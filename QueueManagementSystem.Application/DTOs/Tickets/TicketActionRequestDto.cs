namespace QueueManagementSystem.Application.DTOs.Tickets
{
    public class TicketActionRequestDto
    {
        public int ServiceId { get; set; }
        public int BranchId { get; set; }
        public int? CounterId { get; set; }
    }
}
