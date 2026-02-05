using Microsoft.AspNetCore.SignalR;

namespace QueueManagementSystem.API.Hubs
{
    public class QueueHub : Hub
    {
        public Task JoinServiceGroup(int serviceId, int branchId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, BuildGroupName(serviceId, branchId));
        }

        public Task LeaveServiceGroup(int serviceId, int branchId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, BuildGroupName(serviceId, branchId));
        }

        public static string BuildGroupName(int serviceId, int branchId)
        {
            return $"service-{serviceId}-branch-{branchId}";
        }
    }
}
