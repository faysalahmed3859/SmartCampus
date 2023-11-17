
using Microsoft.AspNetCore.Mvc;
using SmartCampus.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Components
{
    public class CategoryMenu:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CategoryMenu(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories
             .Where(c => c.CategoryStatus == "Enable" && c.Name != "Events" && c.Name != "Notice" && c.Name != "BlogPost")
             .OrderBy(c => c.Name)
             .ToList();

            return View(categories);
        }
        
    }
}
