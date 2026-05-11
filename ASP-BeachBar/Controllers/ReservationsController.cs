using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_BeachBar.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ASP_BeachBar.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Client> _userManager;

        public ReservationsController(ApplicationDbContext context,UserManager<Client> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["EventsId"] = new SelectList(_context.Events.OrderBy(e => e.DateReservation), "Id", "Name");
            }

            var reservations = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Clients)
                .Include(r => r.Events)
                .OrderByDescending(r => r.ReservationDate)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                reservations = reservations.Where(r => r.ClientId == userId);
            }

            return View(await reservations.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ByEvent(int? eventId)
        {
            ViewData["EventsId"] = new SelectList(
                _context.Events.OrderBy(e => e.DateReservation),
                "Id",
                "Name",
                eventId);

            var reservations = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Clients)
                .Include(r => r.Events)
                .OrderByDescending(r => r.ReservationDate)
                .AsQueryable();

            if (eventId.HasValue)
            {
                reservations = reservations.Where(r => r.EventsId == eventId.Value);
            }

            ViewData["SelectedEventId"] = eventId;
            return View("Index", await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservations = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Clients)
                .Include(r => r.Events)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                reservations = reservations.Where(r => r.ClientId == userId);
            }

            var reservation = await reservations.FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(int? eventId)
        {
            ViewData["EventsId"] = new SelectList(AvailableReservationEvents(), "Id", "Name", eventId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventsId,Count")] Reservation reservation)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            reservation.ReservationDate = DateTime.Now;
            reservation.ClientId = userId;
            reservation.Status = ReservationStatus.Active;
            if (reservation.Count <= 0)
            {
                ModelState.AddModelError(nameof(reservation.Count), "Броят места трябва да е поне 1.");
            }

            var selectedEvent = await _context.Events.FindAsync(reservation.EventsId);
            if (selectedEvent == null)
            {
                ModelState.AddModelError(nameof(reservation.EventsId), "Избери валидно събитие.");
            }
            else if (selectedEvent.DateReservation < DateTime.Now)
            {
                ModelState.AddModelError(nameof(reservation.EventsId), "Не може да се резервира минало събитие.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventsId"] = new SelectList(AvailableReservationEvents(), "Id", "Name", reservation.EventsId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservations = _context.Reservations.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                reservations = reservations.Where(r => r.ClientId == userId);
            }

            var reservation = await reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "UserName", reservation.ClientId);
            ViewData["EventsId"] = new SelectList(AvailableReservationEvents(reservation.EventsId), "Id", "Name", reservation.EventsId);
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventsId,Count")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            var existingReservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (existingReservation == null ||
                (!User.IsInRole("Admin") && existingReservation.ClientId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            if (reservation.Count <= 0)
            {
                ModelState.AddModelError(nameof(reservation.Count), "Броят места трябва да е поне 1.");
            }

            var selectedEvent = await _context.Events.FindAsync(reservation.EventsId);
            if (selectedEvent == null)
            {
                ModelState.AddModelError(nameof(reservation.EventsId), "Избери валидно събитие.");
            }
            else if (selectedEvent.DateReservation < DateTime.Now)
            {
                ModelState.AddModelError(nameof(reservation.EventsId), "Не може да се резервира минало събитие.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingReservation.EventsId = reservation.EventsId;
                    existingReservation.Count = reservation.Count;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "UserName", existingReservation.ClientId);
            ViewData["EventsId"] = new SelectList(AvailableReservationEvents(reservation.EventsId), "Id", "Name", reservation.EventsId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservations = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Clients)
                .Include(r => r.Events)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                reservations = reservations.Where(r => r.ClientId == userId);
            }

            var reservation = await reservations.FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null &&
                User.IsInRole("Admin"))
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null ||
                (!User.IsInRole("Admin") && reservation.ClientId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        private IQueryable<Event> AvailableReservationEvents(int? includeEventId = null)
        {
            var now = DateTime.Now;
            return _context.Events
                .Where(e => e.DateReservation >= now || e.Id == includeEventId)
                .OrderBy(e => e.DateReservation);
        }
    }
}
