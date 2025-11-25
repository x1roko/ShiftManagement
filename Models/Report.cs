using System;

namespace ShiftManagement.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalWorkHours { get; set; }
        public int ShiftCount { get; set; }
        public string? Notes { get; set; }
    }
}
