namespace QueueManagementSystem.Application.DTOs.Services
{
    public class ServiceCreateDto
    {
        public string ServiceName { get; set; }
        public int AvgServiceTime { get; set; }
        public int? BranchId { get; set; }
    }
}
