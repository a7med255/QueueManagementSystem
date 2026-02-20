using System;

namespace QueueManagementSystem.Application.DTOs.Statistics
{
    public class ServiceStatisticsDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int TotalCustomersServed { get; set; }
        public double AverageWaitingTimeMinutes { get; set; }
        public TimeSpan? PeakHourStartUtc { get; set; }
        public TimeSpan? PeakHourEndUtc { get; set; }
    }
}
