using AspWebProject.Data;
using AspWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AspWebProject.Controllers
{
    
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        
        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.Service).Include(a => a.Trainer).Include(a => a.User).Include(a => a.FitnessCenter);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
            return View();
        }



        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            Console.WriteLine("===== APPOINTMENT CREATE DEBUG =====");

            // LOGIN USER
            appointment.UserId = _userManager.GetUserId(User);
            ModelState.Remove("UserId");

            Console.WriteLine($"UserId = {appointment.UserId}");
            Console.WriteLine($"FitnessCenterId = {appointment.FitnessCenterId}");
            Console.WriteLine($"ServiceId = {appointment.ServiceId}");
            Console.WriteLine($"TrainerId = {appointment.TrainerId}");
            Console.WriteLine($"AppointmentTime = {appointment.Date}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("MODEL STATE INVALID");
                foreach (var m in ModelState)
                    foreach (var err in m.Value.Errors)
                        Console.WriteLine($"ERROR KEY={m.Key} -> {err.ErrorMessage}");

                ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
                return View(appointment);
            }
            appointment.Date = appointment.Date.Date
       .Add(TimeSpan.Parse(appointment.HourString));

            appointment.Status = AppointmentStatus.Pending;

            // ================================
            // 1) TRAINER O SAATTE BAŞKA RANDEVUDA MI?
            // ================================
            bool trainerBusy = _context.Appointments.Any(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date
            );

            if (trainerBusy)
            {
                ModelState.AddModelError("", "Bu eğitmenin bu saatte başka bir randevusu var.");
                ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
                return View(appointment);
            }

            // ================================
            // 2) EĞİTMENİN O GÜN O SAATTE ÇALIŞIP ÇALIŞMADIĞI
            // ================================
            var day = appointment.Date.DayOfWeek;
            var time = appointment.Date.TimeOfDay;

            var availability = _context.TrainerAvailabilities
                .Where(a => a.TrainerId == appointment.TrainerId && a.Day == day)
                .ToList();

            bool isAvailable = availability.Any(a =>
                time >= a.StartTime &&
                time <= a.EndTime
            );

            if (!isAvailable)
            {
                ModelState.AddModelError("", "Bu eğitmen bu saatte çalışmıyor.");
                ViewBag.FitnessCenters = new SelectList(_context.FitnessCenters, "Id", "Name");
                return View(appointment);
            }

            // ================================
            // TÜM KONTROLLER GEÇTİ → KAYDET
            // ================================
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }







        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TrainerId,ServiceId,Date,StartTime,Status")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        // ADMIN ACTIONS onaylama
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null) return NotFound();

            app.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // ADMIN ACTIONS reddetme
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.Appointments.FindAsync(id);
            if (app == null) return NotFound();

            app.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // AJAX: Belirli bir eğitmen, hizmet ve tarihe göre müsait saatleri al
        [HttpGet]
        public async Task<IActionResult> GetAvailableHours(int trainerId, int serviceId, DateTime date)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return BadRequest("Service not found");

            int duration = service.Duration;

            var day = date.DayOfWeek;

            // Eğitmenin o günkü çalışma saatleri
            var availability = await _context.TrainerAvailabilities
                .Where(a => a.TrainerId == trainerId && a.Day == day)
                .ToListAsync();

            if (!availability.Any())
                return Ok(new List<string>());

            List<TimeSpan> possibleHours = new();

            foreach (var a in availability)
            {
                TimeSpan cursor = a.StartTime;

                while (cursor.Add(TimeSpan.FromHours(duration)) <= a.EndTime)
                {
                    possibleHours.Add(cursor);
                    cursor = cursor.Add(TimeSpan.FromHours(1));
                }
            }

            // Aynı gün alınmış randevular
            var existing = await _context.Appointments
                .Where(x => x.TrainerId == trainerId && x.Date.Date == date.Date)
                .ToListAsync();

            List<string> available = new();

            foreach (var hour in possibleHours)
            {
                bool conflict = existing.Any(e =>
                    e.Date.TimeOfDay < hour.Add(TimeSpan.FromHours(duration)) &&
                    e.Date.TimeOfDay.Add(TimeSpan.FromHours(duration)) > hour
                );

                if (!conflict)
                    available.Add(hour.ToString(@"hh\:mm"));
            }

            return Ok(available);
        }


    }
}
