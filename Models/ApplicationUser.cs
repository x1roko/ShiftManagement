using Microsoft.AspNetCore.Identity;

namespace ShiftManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Employee? Employee { get; set; }
    }
}
