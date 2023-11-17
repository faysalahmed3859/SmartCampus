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

namespace SmartCampus.Controllers
{
    public class AcademicResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AcademicResourcesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg , string sortOrder,string searchString)
        {
            ViewBag.productnam = string.IsNullOrEmpty(sortOrder) ? "prod_desc" : "";
            var product = _context.AcademicResources.Include(c => c.Category).Include(d => d.Subject).Include(e => e.Department).Where(c => c.AcademicResourceStatus == "Enable");
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
              product=  _context.AcademicResources.Include(c => c.Category).Include(d => d.Subject).Include(e => e.Department).Where(c => c.AcademicResourceStatus == "Enable"&& c.Title.ToLower().Contains(searchString.ToLower()));
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
                ViewData["SubCategory"] = new SelectList(_context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(), "Id", "Name");
                ViewData["Brand"] = new SelectList(_context.Subjects.Where(c => c.SubjectStatus == "Enable").ToList(), "Id", "Name");
                AcademicResourceVm vm = new AcademicResourceVm();
                return View(vm);
            }

            else
            {
                var product = await _context.AcademicResources.FindAsync(id);
                if (product == null)
                {

                    return NotFound();
                }
               AcademicResourceVm productVm = new AcademicResourceVm();
                var galleries = _context.ImageCollections.Where(c => c.AcademicResourceId == product.Id).ToList();
                var pdfs = _context.PdfCollections.Where(c => c.AcademicResourceId == product.Id).ToList();
                productVm.Id = product.Id;
                productVm.Title = product.Title;
                productVm.Description = product.Description;
                productVm.ImagePath = product.PdfPath;
                productVm.Author = product.Author;
                productVm.Date = product.Date;
                productVm.AcademicResourceStatus = product.AcademicResourceStatus;
                productVm.CategoryId = product.CategoryId;
                productVm.SubjectId = product.SubjectId;
                productVm.DepartmentId = product.DepartmentId;
                productVm.ImageCollectionPath = galleries;
                productVm.PdfCollectionsPath = pdfs;

                ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.CategoryStatus == "Enable").ToList(), "Id", "Name");
                ViewData["SubCategory"] = new SelectList(_context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(), "Id", "Name");
                ViewData["Brand"] = new SelectList(_context.Subjects.Where(c => c.SubjectStatus == "Enable").ToList(), "Id", "Name");
                return View(productVm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(Guid id, AcademicResourceVm productVm)
        {
            if (ModelState.IsValid)
            {
               
                
                AcademicResource entity;
                string uniqueFileNAme = null;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadimages");
                string uploadspdfFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploadpdfs");
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    entity = new AcademicResource();
                    entity.Id = Guid.NewGuid();
                    entity.Title = productVm.Title;
                    entity.Description = productVm.Description;
                    entity.Author = productVm.Author;
                    entity.Date = productVm.Date;
                    entity.AcademicResourceStatus = productVm.AcademicResourceStatus;
                    entity.CategoryId = productVm.CategoryId;
                    entity.SubjectId = productVm.SubjectId;
                    entity.DepartmentId = productVm.DepartmentId;
                    entity.IsApproved = true; // Set IsApproved to true
                    _context.Add(entity);
                    await _context.SaveChangesAsync();


                    if (productVm.ImageCollections != null && productVm.ImageCollections.Count > 0)
                    {
                        foreach (IFormFile image in productVm.ImageCollections)
                        {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                            var img = new ImageCollection();
                            img.ImagePath = "uploadimages/" + uniqueFileNAme;
                            img.AcademicResourceId = entity.Id;
                            _context.ImageCollections.Add(img);

                        }
                        IFormFile primaryImage = productVm.ImageCollections[0];
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
                    if (productVm.PdfCollections != null && productVm.PdfCollections.Count > 0)
                    {
                        foreach (IFormFile image in productVm.PdfCollections)
                        {
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadspdfFolder, uniqueFileNAme);
                            await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                            var img = new PdfCollection();
                            img.PdfPath = "uploadpdfs/" + uniqueFileNAme;
                            img.AcademicResourceId = entity.Id;
                            _context.PdfCollections.Add(img);

                        }
                        IFormFile primaryImage = productVm.PdfCollections[0];
                        uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                        string primaryImgFilePath = Path.Combine(uploadspdfFolder, uniqueFileNAme);
                        await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                        entity.PdfPath = "uploadpdfs/" + uniqueFileNAme;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        entity.PdfPath = "uploadpdfs/noimage.jpg";
                    }

                }

                else
                {
                    try
                    {
                        entity = await _context.AcademicResources.FindAsync(productVm.Id);
                        entity.Title = productVm.Title;
                        entity.Description = productVm.Description;
                        entity.Author = productVm.Author;
                        entity.Date = productVm.Date;
                        entity.AcademicResourceStatus = productVm.AcademicResourceStatus;
                        entity.CategoryId = productVm.CategoryId;
                        entity.SubjectId = productVm.SubjectId;
                        entity.DepartmentId = productVm.DepartmentId;
                        entity.IsApproved = true; // Set IsApproved to true
                        if (productVm.ImageCollections != null && productVm.ImageCollections.Count > 0)
                        {
                            var oldProductImgcheck = _context.ImageCollections.Where(c => c.AcademicResourceId == entity.Id);
                            if (oldProductImgcheck.Count() > 0)
                            {
                                foreach (var item in oldProductImgcheck)
                                {
                                    _context.ImageCollections.Remove(item);
                                }
                            }

                            foreach (IFormFile image in productVm.ImageCollections)
                            {
                                uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                                var img = new ImageCollection();
                                img.ImagePath = "uploadimages/" + uniqueFileNAme;
                                img.AcademicResourceId = entity.Id;
                                _context.ImageCollections.Add(img);

                            }
                            IFormFile primaryImage = productVm.ImageCollections[0];
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                            string primaryImgFilePath = Path.Combine(uploadsFolder, uniqueFileNAme);
                            await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                            entity.PdfPath = "uploadimages/" + uniqueFileNAme;
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
                    try
                    {
                        entity = await _context.AcademicResources.FindAsync(productVm.Id);
                        entity.Title = productVm.Title;
                        entity.Description = productVm.Description;
                        entity.Author = productVm.Author;
                        entity.Date = productVm.Date;
                        entity.AcademicResourceStatus = productVm.AcademicResourceStatus;
                        entity.CategoryId = productVm.CategoryId;
                        entity.SubjectId = productVm.SubjectId;
                        entity.DepartmentId = productVm.DepartmentId;
                        entity.IsApproved = true; // Set IsApproved to true
                        if (productVm.PdfCollections != null && productVm.PdfCollections.Count > 0)
                        {
                            var oldProductImgcheck = _context.PdfCollections.Where(c => c.AcademicResourceId == entity.Id);
                            if (oldProductImgcheck.Count() > 0)
                            {
                                foreach (var item in oldProductImgcheck)
                                {
                                    _context.PdfCollections.Remove(item);
                                }
                            }

                            foreach (IFormFile image in productVm.PdfCollections)
                            {
                                uniqueFileNAme = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filePath = Path.Combine(uploadspdfFolder, uniqueFileNAme);
                                await image.CopyToAsync(new FileStream(filePath, FileMode.Create));
                                var img = new PdfCollection();
                                img.PdfPath = "uploadpdfs/" + uniqueFileNAme;
                                img.AcademicResourceId = entity.Id;
                                _context.PdfCollections.Add(img);

                            }
                            IFormFile primaryImage = productVm.PdfCollections[0];
                            uniqueFileNAme = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                            string primaryImgFilePath = Path.Combine(uploadspdfFolder, uniqueFileNAme);
                            await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                            entity.PdfPath = "uploadpdfs/" + uniqueFileNAme;
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
                var resultProduct = _context.AcademicResources.Include(c => c.Category).Include(d => d.Subject).Include(e => e.Department).Where(c => c.AcademicResourceStatus == "Enable").ToList();
                var resCount = resultProduct.Count();
                ViewBag.TotalRecord = resCount;
                var pager = new Pager(resCount, pg, pageSize);
                int resSkip = (pg - 1) * pageSize;
                ViewBag.Pager = pager;
                var data = resultProduct.Skip(resSkip).Take(pager.PageSize);

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAllAcademicResource", data) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", productVm) });

        }
        
        [HttpGet("/AcademicResources/GetAllSubCategory")]
        public IActionResult GetAllSubCategory(Guid id)
        {
            var subCategory = _context.Subjects.Where(c => c.DepartmentId == id).ToList();
            return Json(subCategory);
        }
       

    }
}
