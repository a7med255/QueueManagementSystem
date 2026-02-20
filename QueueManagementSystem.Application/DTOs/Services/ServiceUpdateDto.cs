namespace QueueManagementSystem.Application.DTOs.Services
{
    public class ServiceUpdateDto
    {
        public string ServiceName { get; set; }
        public int AvgServiceTime { get; set; }
        public bool IsOpen { get; set; }
        public int? BranchId { get; set; }
    }
}
