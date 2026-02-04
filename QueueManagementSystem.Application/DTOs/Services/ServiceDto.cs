namespace QueueManagementSystem.Application.DTOs.Services
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public int AvgServiceTime { get; set; }
        public bool IsOpen { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
