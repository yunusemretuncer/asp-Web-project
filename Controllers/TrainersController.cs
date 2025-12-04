using AspWebProject.Data;
using AspWebProject.Models;
using AspWebProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace AspWebProject.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult GetByService(int serviceId)
        {
            var trainers = _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId)
                .Select(ts => new {
                    ts.Trainer.Id,
                    ts.Trainer.FullName
                })
                .ToList();

            return Json(trainers);
        }


        // =====================================================
        // INDEX
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .ToListAsync();

            return View(list);
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)                 
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service)
                .Include(t => t.Availabilities)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }


        // =====================================================
        // CREATE GET
        // =====================================================
        public IActionResult Create()
        {
            var vm = new TrainerEditViewModel
            {
                Trainer = new Trainer(),
                AllServices = new List<Service>(),
                SelectedServiceIds = new List<int>()
            };

            ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenters, "Id", "Name");
            return View(vm);
        }

        // =====================================================
        // AJAX: FITNESS CENTER’A GÖRE SERVİSLER
        // =====================================================
        public IActionResult GetServicesByCenter(int fitnessCenterId)
        {
            var services = _context.Services
                .Where(s => s.FitnessCenterId == fitnessCenterId)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Json(services);
        }


        // =====================================================
        // CREATE POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerEditViewModel vm)
        {
            Console.WriteLine("===== TRAINER CREATE DEBUG =====");

            // ------------ DEBUG LOG ------------
            Console.WriteLine($"FullName = {vm.Trainer?.FullName}");
            Console.WriteLine($"FitnessCenterId = {vm.Trainer?.FitnessCenterId}");
            Console.WriteLine($"Selected Services Count = {vm.SelectedServiceIds?.Count}");

            if (vm.Availabilities != null)
            {
                int index = 0;
                foreach (var a in vm.Availabilities)
                {
                    Console.WriteLine($"A[{index}] Enabled={a.Enabled}, Day={a.Day}, Start={a.StartTime}, End={a.EndTime}");
                    index++;
                }
            }

            // 1) Enabled olmayan availability alanlarının zorunluluğunu kaldır
            for (int i = 0; i < vm.Availabilities.Count; i++)
            {
                if (!vm.Availabilities[i].Enabled)
                {
                    ModelState.Remove($"Availabilities[{i}].StartTime");
                    ModelState.Remove($"Availabilities[{i}].EndTime");
                }
            }

            // 2) FITNESS CENTER çalışma saatleri kontrolü
            var fc = await _context.FitnessCenters
     .FirstOrDefaultAsync(f => f.Id == vm.Trainer.FitnessCenterId);

            if (fc == null)
            {
                ModelState.AddModelError("", "Fitness Center bulunamadı.");
            }
            else
            {
                TimeSpan fcStart = fc.OpenTime;
                TimeSpan fcEnd = fc.CloseTime;

                foreach (var a in vm.Availabilities.Where(x => x.Enabled))
                {
                    if (a.StartTime < fcStart || a.EndTime > fcEnd)
                    {
                        ModelState.AddModelError("",
                            $"Müsaitlik saati, spor salonu çalışma saatleri dışında! " +
                            $"Salon çalışma saatleri: {fcStart:hh\\:mm} - {fcEnd:hh\\:mm}");
                    }
                }
            }

            // MODELSTATE SON DURUM
            Console.WriteLine($"MODEL STATE VALID = {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("===== MODEL STATE ERRORS =====");
                foreach (var err in ModelState)
                    foreach (var e in err.Value.Errors)
                        Console.WriteLine($"KEY={err.Key} -> {e.ErrorMessage}");

                ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenters, "Id", "Name");
                return View(vm);
            }

            // -----------------------------------------
            // 3) EXPERTISE STRING OLUŞTUR (YENİ EKLEDİK)
            // -----------------------------------------
            var selectedServices = await _context.Services
                .Where(s => vm.SelectedServiceIds.Contains(s.Id))
                .Select(s => s.Name)
                .ToListAsync();

            vm.Trainer.Expertise = string.Join(", ", selectedServices);
            Console.WriteLine("Generated Expertise: " + vm.Trainer.Expertise);

            // -----------------------------------------
            // 4) Trainer kayıt
            // -----------------------------------------
            _context.Trainers.Add(vm.Trainer);
            await _context.SaveChangesAsync();

            // -----------------------------------------
            // 5) TrainerService kayıt
            // -----------------------------------------
            foreach (var serviceId in vm.SelectedServiceIds)
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = vm.Trainer.Id,
                    ServiceId = serviceId
                });
            }

            // -----------------------------------------
            // 6) Availability kayıt
            // -----------------------------------------
            foreach (var a in vm.Availabilities)
            {
                if (a.Enabled)
                {
                    _context.TrainerAvailabilities.Add(new TrainerAvailability
                    {
                        TrainerId = vm.Trainer.Id,
                        Day = a.Day,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }





        // =====================================================
        // EDIT GET
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                .Include(t => t.Availabilities)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            // --- SERVICES ---
            var vm = new TrainerEditViewModel
            {
                Trainer = trainer,
                AllServices = await _context.Services.ToListAsync(),
                SelectedServiceIds = trainer.TrainerServices.Select(ts => ts.ServiceId).ToList(),

                
            };

            // Hiç availability yoksa default 5 gün oluştur (Pazartesi–Cuma)
            for (int i = 0; i < 5; i++)
            {
                var day = (DayOfWeek)(i + 1); // Mon–Fri
                var exist = trainer.Availabilities
                    .FirstOrDefault(a => a.Day == day);

                vm.Availabilities.Add(new AvailabilityInput
                {
                    Day = day,
                    Enabled = exist != null,
                    StartTime = exist?.StartTime ?? TimeSpan.Zero,
                    EndTime = exist?.EndTime ?? TimeSpan.Zero
                });
            }

            ViewData["FitnessCenterId"] =
                new SelectList(_context.FitnessCenters, "Id", "Name", trainer.FitnessCenterId);

            return View(vm);
        }



        // =====================================================
        // EDIT POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerEditViewModel vm)
        {
            if (id != vm.Trainer.Id)
                return NotFound();

            // Disabled olan availability'lerin validation'ını kaldır
            for (int i = 0; i < vm.Availabilities.Count; i++)
            {
                if (!vm.Availabilities[i].Enabled)
                {
                    ModelState.Remove($"Availabilities[{i}].StartTime");
                    ModelState.Remove($"Availabilities[{i}].EndTime");
                }
            }

            // FitnessCenter saat kontrolü
            var fc = await _context.FitnessCenters.FirstOrDefaultAsync(f => f.Id == vm.Trainer.FitnessCenterId);

            if (fc != null)
            {
                foreach (var a in vm.Availabilities.Where(a => a.Enabled))
                {
                    if (a.StartTime < fc.OpenTime || a.EndTime > fc.CloseTime)
                    {
                        ModelState.AddModelError("",
                            $"Müsaitlik saati {fc.OpenTime}-{fc.CloseTime} dışında!");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                vm.AllServices = _context.Services
                    .Where(s => s.FitnessCenterId == vm.Trainer.FitnessCenterId)
                    .ToList();

                ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenters, "Id", "Name", vm.Trainer.FitnessCenterId);
                return View(vm);
            }

            // Trainer güncelle
            _context.Update(vm.Trainer);
            await _context.SaveChangesAsync();

            // Eski services sil → yenilerini ekle
            var oldServices = _context.TrainerServices.Where(ts => ts.TrainerId == vm.Trainer.Id);
            _context.RemoveRange(oldServices);

            foreach (var sid in vm.SelectedServiceIds)
            {
                _context.TrainerServices.Add(new TrainerService
                {
                    TrainerId = vm.Trainer.Id,
                    ServiceId = sid
                });
            }

            // Expertise string güncelle
            var names = _context.Services
                .Where(s => vm.SelectedServiceIds.Contains(s.Id))
                .Select(s => s.Name)
                .ToList();
            vm.Trainer.Expertise = string.Join(", ", names);

            // Eski availability sil + yenisini ekle
            var oldAvail = _context.TrainerAvailabilities.Where(a => a.TrainerId == vm.Trainer.Id);
            _context.RemoveRange(oldAvail);

            foreach (var a in vm.Availabilities)
            {
                if (a.Enabled)
                {
                    _context.TrainerAvailabilities.Add(new TrainerAvailability
                    {
                        TrainerId = vm.Trainer.Id,
                        Day = a.Day,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // DELETE GET
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // =====================================================
        // DELETE POST
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                .Include(t => t.Availabilities)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer != null)
            {
                _context.TrainerServices.RemoveRange(trainer.TrainerServices);
                _context.TrainerAvailabilities.RemoveRange(trainer.Availabilities);
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

