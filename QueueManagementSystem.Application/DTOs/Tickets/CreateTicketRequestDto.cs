namespace QueueManagementSystem.Application.DTOs.Tickets
{
    public class CreateTicketRequestDto
    {
        public int ServiceId { get; set; }
        public int BranchId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
