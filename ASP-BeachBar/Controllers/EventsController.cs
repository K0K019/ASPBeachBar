using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_BeachBar.Data;
using Microsoft.AspNetCore.Authorization;

namespace ASP_BeachBar.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = _context.Events
                .AsNoTracking()
                .OrderBy(e => e.DateReservation)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                events = events.Where(e => e.DateReservation >= DateTime.Now);
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = _context.Events.AsNoTracking().AsQueryable();
            if (!User.IsInRole("Admin"))
            {
                events = events.Where(e => e.DateReservation >= DateTime.Now);
            }

            var @event = await events.FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,Description,DateReservation")] Event @event)
        {
            @event.RegisterOn = DateTime.Now;
            @event.LastUpdatedOn = @event.RegisterOn;
            if (@event.DateReservation <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(@event.DateReservation), "Събитието трябва да бъде в бъдещето.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,Description,DateReservation")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (@event.DateReservation <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(@event.DateReservation), "Събитието трябва да бъде в бъдещето.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    existingEvent.Name = @event.Name;
                    existingEvent.ImageUrl = @event.ImageUrl;
                    existingEvent.Description = @event.Description;
                    existingEvent.DateReservation = @event.DateReservation;
                    existingEvent.LastUpdatedOn = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
