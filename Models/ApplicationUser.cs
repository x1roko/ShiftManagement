using Microsoft.AspNetCore.Identity;

namespace ShiftManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Employee? Employee { get; set; }
    }
}
