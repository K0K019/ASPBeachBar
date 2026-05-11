using ASP_BeachBar.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_BeachBar.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NavigationMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _context.NavigationMenuItems
                .AsNoTracking()
                .Where(item => item.IsActive)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.Text)
                .ToListAsync();

            menuItems = menuItems
                .Where(item => string.IsNullOrWhiteSpace(item.RequiredRole) || User.IsInRole(item.RequiredRole))
                .ToList();

            return View(menuItems);
        }
    }
}
