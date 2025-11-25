using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftManagement.Data;
using ShiftManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftManagement.Controllers
{
    [Authorize]
    public class ShiftScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShiftScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shifts = await _context.ShiftSchedules!
                .Include(s => s.Employee)
                .OrderBy(s => s.ShiftDate)
                .ToListAsync();
            return View(shifts);
        }

        public async Task<IActionResult> MyShifts()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var employee = await _context.Employees!
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null) return NotFound();

            var shifts = await _context.ShiftSchedules!
                .Where(s => s.EmployeeId == employee.Id)
                .OrderBy(s => s.ShiftDate)
                .ToListAsync();

            return View(shifts);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create(int employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ShiftSchedule shift)
        {
            if (ModelState.IsValid)
            {
                _context.ShiftSchedules?.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.ShiftSchedules!.FindAsync(id);
            if (shift == null) return NotFound();
            return View(shift);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ShiftSchedule shift)
        {
            if (id != shift.Id) return NotFound();
            if (ModelState.IsValid)
            {
                shift.UpdatedAt = DateTime.UtcNow;
                _context.Update(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.ShiftSchedules!
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (shift == null) return NotFound();
            return View(shift);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.ShiftSchedules!.FindAsync(id);
            if (shift != null)
            {
                _context.ShiftSchedules?.Remove(shift);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
