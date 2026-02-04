using System;
using System.Collections.Generic;

namespace QueueManagementSystem.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IReadOnlyCollection<string> Roles { get; set; }
    }
}
