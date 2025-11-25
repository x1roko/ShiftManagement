using System;
using System.Collections.Generic;

namespace ShiftManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<ShiftSchedule> Shifts { get; set; } = new List<ShiftSchedule>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
