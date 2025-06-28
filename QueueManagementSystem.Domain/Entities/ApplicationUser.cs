using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
