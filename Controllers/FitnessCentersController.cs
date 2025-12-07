using AspWebProject.Data;
using AspWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspWebProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FitnessCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FitnessCenters
        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessCenters.ToListAsync());
        }

        // GET: FitnessCenters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCenter = await _context.FitnessCenters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessCenter == null)
            {
                return NotFound();
            }

            return View(fitnessCenter);
        }

        // GET: FitnessCenters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessCenters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,OpenTime,CloseTime")] FitnessCenter fitnessCenter)
        {
            if (fitnessCenter.OpenTime >= fitnessCenter.CloseTime)
            {
                ModelState.AddModelError("", "Açılış saati kapanış saatinden büyük olamaz.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fitnessCenter);
        }


        // GET: FitnessCenters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fc = await _context.FitnessCenters.FindAsync(id);
            if (fc == null) return NotFound();

            return View(fc);
        }


        // POST: FitnessCenters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,OpenTime,CloseTime")] FitnessCenter fitnessCenter)
        {
            if (id != fitnessCenter.Id) return NotFound();

            if (fitnessCenter.OpenTime >= fitnessCenter.CloseTime)
            {
                ModelState.AddModelError("", "Açılış saati kapanış saatinden büyük olamaz.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fitnessCenter);
        }


        // GET: FitnessCenters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessCenter = await _context.FitnessCenters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessCenter == null)
            {
                return NotFound();
            }

            return View(fitnessCenter);
        }

        // POST: FitnessCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCenter = await _context.FitnessCenters.FindAsync(id);
            if (fitnessCenter != null)
            {
                _context.FitnessCenters.Remove(fitnessCenter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessCenterExists(int id)
        {
            return _context.FitnessCenters.Any(e => e.Id == id);
        }
    }
}
