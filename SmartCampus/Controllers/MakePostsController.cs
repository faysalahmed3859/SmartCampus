using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SmartCampus.Data;
using SmartCampus.Extensions;
using SmartCampus.Models;
using SmartCampus.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static SmartCampus.Extensions.Helper;

namespace ERPManagementSystem.Areas.Admin.Controllers
{
    
    public class MakePostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MakePostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg, string sortOrder, string searchString)
        {
            ViewBag.productnam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var product = _context.MakePosts.Include(c => c.Category).Where(c => c.MakepostStatus == "Enable");
            switch (sortOrder)
            {
                case "prod_desc":
                    product = product.OrderByDescending(n => n.Title);
                    break;
                default:
                    product = product.OrderBy(n => n.Title);
                    break;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                product = _context.MakePosts.Include(c => c.Category).Where(c => c.MakepostStatus == "Enable" && c.Title.ToLower().Contains(searchString.ToLower()));
            }
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            var resCount = product.Count();
            ViewBag.TotalRecord = resCount;
            var pager = new Pager(resCount, pg, pageSize);
            int resSkip = (pg - 1) * pageSize;
            var data = product.Skip(resSkip).Take(pager.PageSize);
            ViewBag.Pager = pager;

            return View(await data.ToListAsync());

        }
        //AddOrEdit
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.CategoryStatus == "Enable").ToList(), "Id", "Name");
                MakePostVm vm = new MakePostVm();
                return View(vm);
            }

            else
            {
                var product = await _context.MakePosts.FindAsync(id);
                if (product == null)
                {

                    return NotFound();
                }
                MakePostVm productVm = new MakePostVm();
                var galleries = _context.Galleries.Where(c => c.MakePostId == product.Id).ToList();
                productVm.Id = product.Id;
                productVm.Title = product.Title;
                productVm.Link = product.Link;
                productVm.Date = product.Date;
                productVm.Description = product.Description;
                productVm.CategoryId = product.CategoryId;
                productVm.MakepostStatus = product.MakepostStatus;
                productVm.ImagePath = product.ImagePath;
                productVm.GalleryImagesPath = galleries;

                ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.CategoryStatus == "Enable").ToList(), "Id", "Name");
                
                return View(productVm);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, MakePostVm productVm)
        {
            if (ModelState.IsValid)
            {


                MakePost entity;
                string uniqueFileNAme = null;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadimages");
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new MakePost();
                    entity.Id = Guid.NewGuid();
                    entity.Title = productVm.Title;
                    entity.Link = productVm.Link;
                    entity.Date = productVm.Date;
                    entity.Description = productVm.Description;
                    entity.MakepostStatus = productVm.MakepostStatus;
                    entity.CategoryId = productVm.CategoryId;
                    entity.IsApproved = true; // Set IsApproved to true
                    _context.Add(entity);
                    await _context.SaveChangesAsync();


                    if (productVm.Galleries != null && productVm.Galleries.Count > 0)
                    {
                        foreach (IFormFile image in productVm.Galleries)
                        {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                            var img = new Gallery();
                            img.ImagePath = "uploadimages/" + uniqueFileNAme;
                            img.MakePostId = entity.Id;
                            _context.Galleries.Add(img);

                        }
                        IFormFile primaryImage = productVm.Galleries[0];
                        uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                        string primaryImgFilePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                        await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                        entity.ImagePath = "uploadimages/" + uniqueFileNAme;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        entity.ImagePath = "uploadimages/noimage.jpg";
                    }
                }

                else
                {
                    try
                    {
                        entity = await _context.MakePosts.FindAsync(productVm.Id);
                        
                        entity.Title = productVm.Title;
                        entity.Link = productVm.Link;
                        entity.Date = productVm.Date;
                        entity.Description = productVm.Description;
                        entity.MakepostStatus = productVm.MakepostStatus;
                        entity.CategoryId = productVm.CategoryId;
                        entity.IsApproved = true; // Set IsApproved to true
                        if (productVm.Galleries != null && productVm.Galleries.Count > 0)
                        {
                            var oldProductImgcheck = _context.Galleries.Where(c => c.MakePostId == entity.Id);
                            if (oldProductImgcheck.Count() > 0)
                            {
                                foreach (var item in oldProductImgcheck)
                                {
                                    _context.Galleries.Remove(item);
                                }
                            }

                            foreach (IFormFile image in productVm.Galleries)
                            {
                                uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                                var img = new Gallery();
                                img.ImagePath = "uploadimages/" + uniqueFileNAme;
                                img.MakePostId = entity.Id;
                                _context.Galleries.Add(img);

                            }
                            IFormFile primaryImage = productVm.Galleries[0];
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                            string primaryImgFilePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                            entity.ImagePath = "uploadimages/" + uniqueFileNAme;
                        }
                        //else
                        //{
                        //    entity.ImagePath = "uploadimages/noimage.jpg"; 
                        //}

                        _context.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                    }
                }
                const int pageSize = 10;
                int pg = 1;
                if (pg < 1)
                {
                    pg = 1;
                }
                var resultProduct = _context.MakePosts.Include(c => c.Category).Where(c => c.MakepostStatus == "Enable").ToList();
                var resCount = resultProduct.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                ViewBag.Pager = pager;
                var data = resultProduct.Skip(resSkip).Take(pager.PageSize);

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllMakePost", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", productVm) });

        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var banner = await _context.MakePosts.FindAsync(id);
            banner.MakepostStatus = "Disable";
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllMakeMost", _context.MakePosts.Include(c => c.Category).Where(c => c.MakepostStatus == "Enable").ToList()) });

        }

    }
}
