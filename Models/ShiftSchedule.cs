using System;

namespace ShiftManagement.Models
{
    public class ShiftSchedule
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        
        public DateTime ShiftDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ShiftStatus Status { get; set; } = ShiftStatus.Planned;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum ShiftStatus
    {
        Planned,
        Completed,
        Cancelled,
        Modified
    }
}
