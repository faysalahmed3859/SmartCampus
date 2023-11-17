using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SmartCampus.Data;
using SmartCampus.Extensions;
using SmartCampus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static SmartCampus.Extensions.Helper;

namespace SmartCampus.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
       
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
           
        }

        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            //var message = new Message(new string[] { "faysal@gmail.com" }, "Test email async", "This is the content from our async email.", null);
            //ViewBag.srcString = searchString;
            ViewBag.departmentnam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var department = _context.Departments.Where(c => c.DepartmentStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    department = department.OrderByDescending(n => n.Name);
                    break;
                default:
                    department = department.OrderBy(n => n.Name);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                department = _context.Departments.Where(c => c.DepartmentStatus == "Enable" && c.Name.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = department.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = department.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;
            return View(await data.ToListAsync());
        }

        //AddOrEdit
        [NoDirectAccess]
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                return View(new Department());
            }

            else
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound();
                }
                return View(department);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, Department department)
        {
            if (ModelState.IsValid)
            {
                Department entity ;
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new Department();
                    entity.Id = Guid.NewGuid();
                    entity.Name = department.Name;
                    entity.DepartmentStatus = department.DepartmentStatus;
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                }

                else
                {
                    try
                    {
                        entity =await _context.Departments.FindAsync(department.Id);
                        entity.Name = department.Name;
                        entity.DepartmentStatus = department.DepartmentStatus;
                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        
                    }
                }
                var departmentData = _context.Departments.Where(c => c.DepartmentStatus == "Enable");
                int pg=1;
                const int pageSize = 5;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resCount = departmentData.Count();
                ViewBag.TotalRecord = resCount;
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                var data = departmentData.Skip(resSkip).Take(pager.PageSize);
                ViewBag.Pager = pager;
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllDepartment", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", department) });

        }
        //delete category
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var department = await _context.Departments.FindAsync(id);
            department.DepartmentStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllDepartment", _context.Departments.Where(c=>c.DepartmentStatus == "Enable").ToList()) });

        }
    }
}
