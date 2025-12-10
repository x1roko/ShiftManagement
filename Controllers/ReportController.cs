using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftManagement.Data;
using ShiftManagement.Models;
using ShiftManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftManagement.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExportService _exportService;

        public ReportController(ApplicationDbContext context, ExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var reports = await _context.Reports
                .Include(r => r.Employee)
                .ToListAsync();
            return View(reports);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Generate()
        {
            ViewBag.Employees = await _context.Employees.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Generate(int employeeId, DateTime startDate, DateTime endDate)
        {
            var shifts = await _context.ShiftSchedules
                .Where(s => s.EmployeeId == employeeId && 
                            s.ShiftDate >= startDate && 
                            s.ShiftDate <= endDate &&
                            s.Status == ShiftStatus.Completed)
                .ToListAsync();

            var totalHours = shifts.Sum(s => (s.EndTime - s.StartTime).TotalHours);

            var report = new Report
            {
                EmployeeId = employeeId,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalWorkHours = (decimal)totalHours,
                ShiftCount = shifts.Count,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportCsv(int? reportId)
        {
            var report = await _context.Reports
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return NotFound();

            var content = _exportService.ExportReportToCsv(report);
            return File(content, "text/csv", $"Report_{report.Id}.csv");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportExcel(int? reportId)
        {
            var report = await _context.Reports
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null) return NotFound();

            var content = _exportService.ExportReportToExcel(report);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Report_{report.Id}.xlsx");
        }
    }
}
