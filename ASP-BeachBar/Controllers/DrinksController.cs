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
    public class DrinksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DrinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Drinks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drinks
                .AsNoTracking()
                .Include(d => d.Categories)
                .OrderBy(d => d.Categories.Name)
                .ThenBy(d => d.Name)
                .ToListAsync());
        }

        // GET: Drinks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drinks
                .AsNoTracking()
                .Include(d => d.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drink == null)
            {
                return NotFound();
            }

            return View(drink);
        }

        // GET: Drinks/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,IsAlcoholic,CategoryId,Weight,Price")] Drink drink)
        {
            drink.RegisterOn = DateTime.Now;
            await ValidateCategoryAsync(drink.CategoryId);

            if (ModelState.IsValid)
            {
                _context.Add(drink);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", drink.CategoryId);
            return View(drink);
        }

        // GET: Drinks/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drinks.FindAsync(id);
            if (drink == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", drink.CategoryId);
            return View(drink);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,IsAlcoholic,CategoryId,Weight,Price")] Drink drink)
        {
            if (id != drink.Id)
            {
                return NotFound();
            }

            await ValidateCategoryAsync(drink.CategoryId);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDrink = await _context.Drinks.FirstOrDefaultAsync(d => d.Id == id);
                    if (existingDrink == null)
                    {
                        return NotFound();
                    }

                    existingDrink.Name = drink.Name;
                    existingDrink.ImageUrl = drink.ImageUrl;
                    existingDrink.IsAlcoholic = drink.IsAlcoholic;
                    existingDrink.CategoryId = drink.CategoryId;
                    existingDrink.Weight = drink.Weight;
                    existingDrink.Price = drink.Price;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrinkExists(drink.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", drink.CategoryId);
            return View(drink);
        }

        // GET: Drinks/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drinks
                .AsNoTracking()
                .Include(d => d.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drink == null)
            {
                return NotFound();
            }

            return View(drink);
        }

        // POST: Drinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drink = await _context.Drinks.FindAsync(id);
            if (drink != null)
            {
                _context.Drinks.Remove(drink);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrinkExists(int id)
        {
            return _context.Drinks.Any(e => e.Id == id);
        }

        private async Task ValidateCategoryAsync(int categoryId)
        {
            if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
            {
                ModelState.AddModelError(nameof(Drink.CategoryId), "Избери валидна категория.");
            }
        }
    }
}
