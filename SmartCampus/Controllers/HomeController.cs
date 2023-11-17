using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartCampus.Data;
using SmartCampus.Models;
using System;
using cloudscribe.Pagination.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SmartCampus.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SmartCampus.Extensions;
using static SmartCampus.Extensions.Helper;

namespace SmartCampus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly IServiceProvider services;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, 
            IServiceProvider services,UserManager<ApplicationUser>userManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.services = services;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var academice = GetAcademice();
            var banner = GetBanner();
            IndexVM model = new IndexVM();
            model.Banners = banner;
            model.AcademicResources = academice;

            return View(model);
        }
        private List<AcademicResource> GetAcademice()
        {

            return (context.AcademicResources.Include(c => c.Category)
                .Include(d => d.Department).Include(e => e.Subject)
                .Where(c => c.AcademicResourceStatus == "Enable" && c.IsApproved==true)
                .OrderByDescending(c => c.Date).ToList());
        }
        private List<Banner> GetBanner()
        {
            var banner = context.Banners.Where(c => c.BannerStatus == "Enable");
            return (banner).ToList();
        }
        
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllResourceByCategory(int pg, Guid categoryId, int pageNumber = 1, int pageSize = 9)
        {
            // Inside the AllAcademiceResource action
            ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var product = context.AcademicResources.Where(c => c.CategoryId == categoryId).Include(d => d.Department)
                .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable" && c.IsApproved == true)
                .OrderByDescending(c => c.Date)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<AcademicResource>
            {
                Data = product.AsNoTracking().ToList(),
                TotalItems = context.MakePosts.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }
        [HttpPost]
        public IActionResult GetAllResourceByCategory(Guid departmentId, Guid subjectId, int pg, Guid categoryId, int pageNumber = 1, int pageSize = 9)
        {
            // Inside the AllAcademiceResource action
            ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var product = context.AcademicResources.Where(c => c.CategoryId == categoryId).Include(d => d.Department)
                .Include(e => e.Subject).Where(r => r.DepartmentId == departmentId && r.SubjectId == subjectId
                &&  r.AcademicResourceStatus == "Enable" && r.IsApproved == true)
                .OrderByDescending(c => c.Date)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<AcademicResource>
            {
                Data = product.AsNoTracking().ToList(),
                TotalItems = context.MakePosts.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }
        public IActionResult AllAcademiceResource(int pageNumber = 1, int pageSize = 9)
        {

            // Inside the AllAcademiceResource action
            ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();

            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var resource = context.AcademicResources.Include(c => c.Category).Include(d => d.Department)
                .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable" && c.IsApproved == true)
                .Skip(ExcludeRecords)
                .OrderByDescending(c => c.Date)
                .Take(pageSize);
            var result = new PagedResult<AcademicResource>
            {
                Data = resource.AsNoTracking().ToList(),
                TotalItems = context.AcademicResources.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);

        }


        public IActionResult GetSubjectsByDepartment(Guid departmentId)
        {
            var subjects = context.Subjects.Where(s => s.DepartmentId == departmentId && s.SubjectStatus=="Enable").ToList();
            return PartialView("_SubjectsDropdownPartial", subjects);
        }
        [HttpPost]
        public IActionResult AllAcademiceResource(Guid departmentId, Guid subjectId, int pageNumber = 1, int pageSize = 9)
        {

            // Inside the AllAcademiceResource action
            ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();

            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var resource = context.AcademicResources
                .Where(r => r.DepartmentId == departmentId && r.SubjectId == subjectId 
                && r.AcademicResourceStatus == "Enable" && r.IsApproved == true)
                .Skip(ExcludeRecords)
                .OrderByDescending(c => c.Date)
                .Take(pageSize);
            var result = new PagedResult<AcademicResource>
            {
                Data = resource.AsNoTracking().ToList(),
                TotalItems = context.AcademicResources.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }


       
        //Get AllResource detail acation method
        public ActionResult Details(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var product = context.AcademicResources.Include(c => c.Category).Include(d => d.Department)
                .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable" && c.IsApproved == true).FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.gallery = context.ImageCollections.Where(c => c.AcademicResourceId == product.Id);
            return View(product);
        }

        //POST product detail acation method
        [HttpPost]
        [ActionName("Details")]
        public ActionResult ProductDetails(Guid id, AcademicResource pro)
        {
            List<AcademicResource> products = new List<AcademicResource>();
            if (id == null)
            {
                return NotFound();
            }

            var product = context.AcademicResources.Include(c => c.Category).Include(d => d.Department)
                 .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable" && c.IsApproved == true).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
         
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

        public IActionResult GetCategory(int pageNumber = 1, int pageSize = 9)
        {


            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var resource = context.MakePosts.Include(c => c.Category).
                Where(c => c.Category.Name == "Notice" && c.Category.CategoryStatus=="Enable" && c.MakepostStatus == "Enable" && c.IsApproved ==true)
                .OrderByDescending(c => c.Date)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<MakePost>
            {
                Data = resource.AsNoTracking().ToList(),
                TotalItems = context.AcademicResources.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }
        //Get AllResource detail acation method
        public ActionResult GetCategoryDetails(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var product = context.MakePosts.Include(c => c.Category)
                .Where(c => c.MakepostStatus == "Enable" && c.IsApproved == true).FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.gallery = context.Galleries.Where(c => c.MakePostId == product.Id);
            return View(product);
        }
        public IActionResult ListByCategory(int pageNumber = 1, int pageSize = 9)
        {


            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var resource = context.MakePosts.Include(c => c.Category).
                Where(c => c.Category.Name == "Events" && c.Category.CategoryStatus == "Enable" && c.MakepostStatus == "Enable" && c.IsApproved == true)
                .OrderByDescending(c => c.Date)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<MakePost>
            {
                Data = resource.AsNoTracking().ToList(),
                TotalItems = context.MakePosts.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }

        //AddOrEdit
        public async Task<IActionResult> AddOrEdit(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewData["Category"] = new SelectList(context.Categories
                    .Where(c => c.CategoryStatus == "Enable" && c.Name =="Events"|| c.Name=="BlogPost"), "Id", "Name");
                MakePostVm vm = new MakePostVm();
                return View(vm);
            }

            else
            {
                var product = await context.MakePosts.FindAsync(id);
                if (product == null)
                {

                    return NotFound();
                }
                // Get the user by their ID
                var user = await userManager.FindByIdAsync(product.UserId);

                if (user != null)
                {
                    if (User.Identity.IsAuthenticated && user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    {
                        MakePostVm productVm = new MakePostVm();
                        var galleries = context.Galleries.Where(c => c.MakePostId == product.Id).ToList();
                        productVm.Id = product.Id;
                        productVm.Title = product.Title;
                        productVm.Link = product.Link;
                        productVm.Date = product.Date;
                        productVm.Description = product.Description;
                        productVm.CategoryId = product.CategoryId;
                        productVm.MakepostStatus = product.MakepostStatus;
                        productVm.ImagePath = product.ImagePath;
                        productVm.GalleryImagesPath = galleries;
                        ViewData["Category"] = new SelectList(context.Categories
                            .Where(c => c.CategoryStatus == "Enable" && c.Name == "Events" || c.Name == "BlogPost"), "Id", "Name");
                        return View(productVm);
                    }

                }

            }
            // Redirect or show an unauthorized view for users who don't own the post
            return View("Unauthorized"); // Create a custom unauthorized view
        }



        [HttpPost]
        public async Task<IActionResult> AddOrEdit(Guid id, MakePostVm productVm)
        {
            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploadimages");

                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    // Creating a new MakePost record
                    var entity = new MakePost
                    {
                        Id = Guid.NewGuid(),
                        Title = productVm.Title,
                        Link = productVm.Link,
                        Date = productVm.Date,
                        Description = productVm.Description,
                        MakepostStatus = productVm.MakepostStatus,
                        CategoryId = productVm.CategoryId,
                        IsApproved = false, // Set IsApproved to false
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    };

                    context.Add(entity);
                    await context.SaveChangesAsync();

                    await HandleImageUploads(entity, productVm.Galleries, uploadsFolder);

                    // Redirect to a different action or view after successfully creating the MakePost
                    return RedirectToAction("Sussesful");
                }
                else
                {
                    try
                    {
                        var entity = await context.MakePosts.FindAsync(productVm.Id);
                        if (entity == null)
                        {
                            return NotFound();
                        }

                        entity.Title = productVm.Title;
                        entity.Link = productVm.Link;
                        entity.Date = productVm.Date;
                        entity.Description = productVm.Description;
                        entity.MakepostStatus = productVm.MakepostStatus;
                        entity.CategoryId = productVm.CategoryId;
                        entity.IsApproved = false; // Set IsApproved to false

                        context.Update(entity);
                        await context.SaveChangesAsync();

                        await HandleImageUploads(entity, productVm.Galleries, uploadsFolder);

                        // Redirect to a different action or view after successfully editing the MakePost
                        return RedirectToAction("Sussesful");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // Handle the exception or log it
                        ModelState.AddModelError("", "Concurrency error occurred. Please try again.");
                    }
                }
            }

            // If the model state is not valid, return to the AddOrEdit view with validation errors
            return View(productVm);
        }

        private async Task HandleImageUploads(MakePost entity, List<IFormFile> images, string uploadsFolder)
        {
            if (images != null && images.Count > 0)
            {
                // Handle image uploads
                foreach (var image in images)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Add debugging output to check the file path
                    Console.WriteLine("File Path: " + filePath);

                    try
                    {
                        await image.CopyToAsync(new FileStream(filePath, FileMode.Create));

                        var img = new Gallery
                        {
                            ImagePath = "uploadimages/" + uniqueFileName,
                            MakePostId = entity.Id
                        };

                        context.Galleries.Add(img);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log it
                        Console.WriteLine("Error: " + ex.Message);
                        // You can also add ModelState errors for better user feedback
                        ModelState.AddModelError("", "Error occurred while uploading images.");
                    }
                }

                // Update the primary image path
                var primaryImage = images[0];
                string primaryUniqueFileName = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                string primaryImgFilePath = Path.Combine(uploadsFolder, primaryUniqueFileName);

                try
                {
                    await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                    entity.ImagePath = "uploadimages/" + primaryUniqueFileName;
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Handle the exception or log it
                    Console.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "Error occurred while uploading the primary image.");
                }
            }
            else
            {
                // Handle case when there are no images
                entity.ImagePath = "uploadimages/noimage.jpg";
                await context.SaveChangesAsync();
            }
        }


        // Approve a post (Admin action)
        [HttpGet]
        public async Task<IActionResult> Sussesful(Guid id)
        {

            return View(); // Handle the case where the post is not found
        }
        [HttpGet]
        public async Task<IActionResult> ApprovePost(Guid id)
        {
            var post = await context.MakePosts.FindAsync(id);
            if (post != null)
            {
                return View(post); // Return a view for approving the post
            }
            return NotFound(); // Handle the case where the post is not found
        }


        [HttpPost]
        [ActionName("ApprovePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePosts(Guid id)
        {
            var post = await context.MakePosts.FindAsync(id);
            if (post != null)
            {
                post.IsApproved = true; // Approve the post
                await context.SaveChangesAsync();
                return RedirectToAction("ApproveIndex");
            }
            return NotFound(); // Handle the case where the post is not found
        }
        [HttpGet]
        public IActionResult ApproveDetails(Guid id)
        {
            var post = context.MakePosts
                .Include(c => c.Category)
                .FirstOrDefault(p => p.Id == id && p.IsApproved == false); // Ensure the post is approved

            if (post == null)
            {
                return NotFound(); // Handle the case where the post is not found or not approved
            }

            return View(post);
        }
        public IActionResult ApproveIndex()
        {
            var posts = context.MakePosts.Include(c => c.Category).OrderByDescending(c => c.Date)
                .Where(c => c.IsApproved == false).ToList();
            return View(posts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Retrieve the MakePost entity
            var makePost = await context.MakePosts.FindAsync(id);

            if (makePost == null)
            {
                return NotFound(); // Return a not found response if the post doesn't exist
            }

            // Check if the current user is the owner of the post
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (makePost.UserId != userId)
            {
                return Forbid(); // Return a forbidden response if the user doesn't own the post
            }

            // Delete the MakePost entity
            context.MakePosts.Remove(makePost);
            await context.SaveChangesAsync();

            // Redirect to a different view or action after successful deletion
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult MakeIndex()
        {
            var makePosts = context.MakePosts.Include(c => c.Category).Where(c => c.MakepostStatus == "Enable").ToList();
            return View(makePosts);
        }

        public IActionResult UserPosts(string userId)
        {
            var academicResources = context.AcademicResources
                .Include(c => c.Category)
                .Include(d => d.Subject)
                .Include(e => e.Department)
                 .OrderByDescending(c => c.Date)
                .Where(p => p.UserId == userId && p.AcademicResourceStatus == "Enable" && p.IsApproved == true)
                .ToList();

            var userPosts = context.MakePosts
                .Include(c => c.Category)
                 .OrderByDescending(c => c.Date)
                .Where(p => p.UserId == userId && p.MakepostStatus == "Enable" && p.IsApproved == true)
                .ToList();

            UserProfileViewModel model = new UserProfileViewModel();

            model.AcademicResources = academicResources ?? new List<AcademicResource>();
            model.Posts = userPosts ?? new List<MakePost>();

            return View(model);
        }

        public IActionResult ViewUserPosts(string userId)
        {
            // Retrieve posts created by the specified user
            var userPosts = context.AcademicResources
                 .OrderByDescending(c => c.Date)
                .Where(p => p.UserId == userId)
                .ToList();

            // Optionally, you can also retrieve information about the target user
            var targetUser = userManager.FindByIdAsync(userId).Result;

            // Pass the userPosts and targetUser to the view
            var viewModel = new UserPostsViewModel
            {
                TargetUser = targetUser,
                AcademicResources = userPosts
            };

            return View(viewModel); // Return the view with the viewModel
        }

        public IActionResult UserProfile(string id)
        {
            var user = userManager.FindByIdAsync(id).Result;

            if (user != null)
            {
                // Retrieve the user's posts from the database
                var posts = context.MakePosts.Include(c => c.Category)
                    .Where(p => p.UserId == id && p.MakepostStatus == "Enable")
                    .OrderByDescending(c => c.Date).ToList();
                var academicResources = context.AcademicResources.Include(c => c.Category)
                .Include(d => d.Subject).Include(e => e.Department)
                .Where(p => p.UserId == id && p.AcademicResourceStatus == "Enable")
                .OrderByDescending(c => c.Date).ToList();
                var viewModel = new UserProfileViewModel
                {
                    User = user,
                    Posts = posts,
                    AcademicResources=academicResources
                };

                return View(viewModel);
            }

            return NotFound();
        }
        public IActionResult UserViewProfile(string id)
        {
            var user = userManager.FindByIdAsync(id).Result;

            if (user != null)
            {
                // Retrieve the user's posts from the database
                var posts = context.MakePosts.Include(c => c.Category)
                    .Where(p => p.UserId == id && p.MakepostStatus == "Enable")
                    .OrderByDescending(c => c.Date).ToList();
                var academicResources = context.AcademicResources.Include(c => c.Category)
                .Include(d => d.Subject).Include(e => e.Department)
                .Where(p => p.UserId == id && p.AcademicResourceStatus == "Enable")
                .OrderByDescending(c => c.Date).ToList();
                var viewModel = new UserProfileViewModel
                {
                    User = user,
                    Posts = posts,
                    AcademicResources = academicResources
                };

                return View(viewModel);
            }

            return NotFound();
        }

        public async Task<IActionResult> StudentIndex()
        {
            var users = userManager.Users;
            return View(users);
        }


        //AddOrEdit
        public async Task<IActionResult> AddOrEditResorce(Guid id)
        {
            if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();
                ViewData["Category"] = new SelectList(context.Categories.Where(c => c.CategoryStatus == "Enable").ToList(), "Id", "Name");
                ViewData["SubCategory"] = new SelectList(context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(), "Id", "Name");
                ViewData["Brand"] = new SelectList(context.Subjects.Where(c => c.SubjectStatus == "Enable").ToList(), "Id", "Name");
                AcademicResourceVm vm = new AcademicResourceVm();
                return View(vm);
            }

            else
            {
                var product = await context.AcademicResources.FindAsync(id);
                if (product == null)
                {

                    return NotFound();
                }                // Get the user by their ID
                var user = await userManager.FindByIdAsync(product.UserId);

                if (user != null)
                {
                    if (User.Identity.IsAuthenticated && user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    {
                        AcademicResourceVm productVm = new AcademicResourceVm();
                        var galleries = context.ImageCollections.Where(c => c.AcademicResourceId == product.Id).ToList();
                        var pdfs = context.PdfCollections.Where(c => c.AcademicResourceId == product.Id).ToList();
                        productVm.Id = product.Id;
                        productVm.Title = product.Title;
                        productVm.Description = product.Description;
                        productVm.ImagePath = product.ImagePath;
                        productVm.PdfPath = product.PdfPath;
                        productVm.Author = product.Author;
                        productVm.Date = product.Date;
                        productVm.AcademicResourceStatus = product.AcademicResourceStatus;
                        productVm.CategoryId = product.CategoryId;
                        productVm.SubjectId = product.SubjectId;
                        productVm.DepartmentId = product.DepartmentId;
                        productVm.ImageCollectionPath = galleries;
                        productVm.PdfCollectionsPath = pdfs;
                        ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();
                        ViewData["Category"] = new SelectList(context.Categories.Where(c => c.CategoryStatus == "Enable").ToList(), "Id", "Name");
                        ViewData["SubCategory"] = new SelectList(context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(), "Id", "Name");
                        ViewData["Brand"] = new SelectList(context.Subjects.Where(c => c.SubjectStatus == "Enable").ToList(), "Id", "Name");
                        return View(productVm);
                    }
                }
            }
            // Redirect or show an unauthorized view for users who don't own the post
            return View("Unauthorized"); // Create a custom unauthorized view
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEditResorce(Guid id, AcademicResourceVm productVm)
        {
            if (ModelState.IsValid)
            {
                // Inside the AllAcademiceResource action
                ViewBag.Departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList();
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploadimages");
                string uploadspdfFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploadpdfs");
                if (id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    // Creating a new MakePost record
                    var entity = new AcademicResource
                    {
                        Id = Guid.NewGuid(),
                        Title = productVm.Title,
                        Description = productVm.Description,
                        Author = productVm.Author,
                        Date = productVm.Date,
                        AcademicResourceStatus = productVm.AcademicResourceStatus,
                        CategoryId = productVm.CategoryId,
                        SubjectId = productVm.SubjectId,
                        DepartmentId = productVm.DepartmentId,
                        IsApproved = false, // Set IsApproved to false
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    };
                    context.Add(entity);
                    await context.SaveChangesAsync();

                    await HandleImageUpload(entity, productVm.ImageCollections, uploadsFolder);
                    await HandlePdfUpload(entity, productVm.PdfCollections, uploadspdfFolder);
                    // Redirect to a different action or view after successfully creating the MakePost
                    return RedirectToAction("Sussesful");
                }
                else
                {
                    try
                    {
                        var entity = await context.AcademicResources.FindAsync(productVm.Id);
                        if (entity == null)
                        {
                            return NotFound();
                        }

                        entity.Title = productVm.Title;
                        entity.Description = productVm.Description;
                        entity.Author = productVm.Author;
                        entity.Date = productVm.Date;
                        entity.AcademicResourceStatus = productVm.AcademicResourceStatus;
                        entity.CategoryId = productVm.CategoryId;
                        entity.SubjectId = productVm.SubjectId;
                        entity.DepartmentId = productVm.DepartmentId;
                        entity.IsApproved = false; // Set IsApproved to false
                        context.Update(entity);
                        await context.SaveChangesAsync();

                        await HandleImageUpload(entity, productVm.ImageCollections, uploadsFolder);
                        await HandlePdfUpload(entity, productVm.PdfCollections, uploadspdfFolder);
                        
                        // Redirect to a different action or view after successfully editing the MakePost
                        return RedirectToAction("Sussesful");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // Handle the exception or log it
                        ModelState.AddModelError("", "Concurrency error occurred. Please try again.");
                    }
                }
                
                
            }
            // If the model state is not valid, return to the AddOrEdit view with validation errors
            return View(productVm);
        }


        
        private async Task HandleImageUpload(AcademicResource entity, List<IFormFile> images, string uploadsFolder)
        {
            if (images != null && images.Count > 0)
            {
                // Handle image uploads
                foreach (var image in images)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Add debugging output to check the file path
                    Console.WriteLine("File Path: " + filePath);

                    try
                    {
                        await image.CopyToAsync(new FileStream(filePath, FileMode.Create));

                        var img = new ImageCollection
                        {
                            ImagePath = "uploadimages/" + uniqueFileName,
                            AcademicResourceId = entity.Id
                        };

                        context.ImageCollections.Add(img);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log it
                        Console.WriteLine("Error: " + ex.Message);
                        // You can also add ModelState errors for better user feedback
                        ModelState.AddModelError("", "Error occurred while uploading images.");
                    }
                }

                // Update the primary image path
                var primaryImage = images[0];
                string primaryUniqueFileName = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                string primaryImgFilePath = Path.Combine(uploadsFolder, primaryUniqueFileName);

                try
                {
                    await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                    entity.ImagePath = "uploadimages/" + primaryUniqueFileName;
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Handle the exception or log it
                    Console.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "Error occurred while uploading the primary image.");
                }
            }
            else
            {
                // Handle case when there are no images
                entity.ImagePath = "uploadimages/noimage.jpg";
                await context.SaveChangesAsync();
            }
        }
        private async Task HandlePdfUpload(AcademicResource entity, List<IFormFile> pdfs, string uploadsFolder)
        {
            if (pdfs != null && pdfs.Count > 0)
            {
                // Handle image uploads
                foreach (var pdf in pdfs)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + pdf.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Add debugging output to check the file path
                    Console.WriteLine("File Path: " + filePath);

                    try
                    {
                        await pdf.CopyToAsync(new FileStream(filePath, FileMode.Create));

                        var img = new PdfCollection
                        {
                            PdfPath = "uploadpdfs/" + uniqueFileName,
                            AcademicResourceId = entity.Id
                        };

                        context.PdfCollections.Add(img);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log it
                        Console.WriteLine("Error: " + ex.Message);
                        // You can also add ModelState errors for better user feedback
                        ModelState.AddModelError("", "Error occurred while uploading images.");
                    }
                }

                // Update the primary image path
                var primaryImage = pdfs[0];
                string primaryUniqueFileName = Guid.NewGuid().ToString() + "_" + primaryImage.FileName;
                string primaryImgFilePath = Path.Combine(uploadsFolder, primaryUniqueFileName);

                try
                {
                    await primaryImage.CopyToAsync(new FileStream(primaryImgFilePath, FileMode.Create));
                    entity.PdfPath = "uploadpdfs/" + primaryUniqueFileName;
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Handle the exception or log it
                    Console.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "Error occurred while uploading the primary image.");
                }
            }
            else
            {
                // Handle case when there are no images
                entity.PdfPath = "uploadpdfs/noimage.jpg";
                await context.SaveChangesAsync();
            }
        }

        [HttpGet("/Home/GetAllSubCategory")]
        public IActionResult GetAllSubCategory(Guid id)
        {
            var subCategory = context.Subjects.Where(c => c.DepartmentId == id).ToList();
            return Json(subCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResorce(Guid id)
        {
            // Retrieve the MakePost entity
            var academicResources = await context.AcademicResources.FindAsync(id);

            if (academicResources == null)
            {
                return NotFound(); // Return a not found response if the post doesn't exist
            }

            // Check if the current user is the owner of the post
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (academicResources.UserId != userId)
            {
                return Forbid(); // Return a forbidden response if the user doesn't own the post
            }

            // Delete the MakePost entity
            context.AcademicResources.Remove(academicResources);
            await context.SaveChangesAsync();

            // Redirect to a different view or action after successful deletion
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ApproveAcademicePost(Guid id)
        {
            var post = await context.AcademicResources.FindAsync(id);
            if (post != null)
            {
                return View(post); // Return a view for approving the post
            }
            return NotFound(); // Handle the case where the post is not found
        }


        [HttpPost]
        [ActionName("ApproveAcademicePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAcademicePosts(Guid id)
        {
            var post = await context.AcademicResources.FindAsync(id);
            if (post != null)
            {
                post.IsApproved = true; // Approve the post
                await context.SaveChangesAsync();
                return RedirectToAction("ApproveAcademiceIndex");
            }
            return NotFound(); // Handle the case where the post is not found
        }
        [HttpGet]
        public IActionResult ApproveAcademiceDetails(Guid id)
        {
            var post = context.AcademicResources.Include(c => c.Category)
                .Include(d => d.Subject).Include(e => e.Department).Where(c => c.AcademicResourceStatus == "Enable")
                .FirstOrDefault(p => p.Id == id && p.IsApproved == false); // Ensure the post is approved

            if (post == null)
            {
                return NotFound(); // Handle the case where the post is not found or not approved
            }

            return View(post);
        }
        public IActionResult ApproveAcademiceIndex()
        {
            var posts = context.AcademicResources.Include(c => c.Category)
                .Include(d => d.Subject).Include(e => e.Department)
                .Where(c => c.IsApproved == false && c.AcademicResourceStatus == "Enable")
                .OrderByDescending(c => c.Date).ToList();
            return View(posts);
        }
        
        public IActionResult BlogPost(int pageNumber = 1, int pageSize = 9)
        {


            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var resource = context.MakePosts.Include(c => c.Category).
                Where(c => c.Category.Name == "BlogPost" && c.Category.CategoryStatus == "Enable" && c.MakepostStatus == "Enable" && c.IsApproved == true)
                .OrderByDescending(c => c.Date)
                .Skip(ExcludeRecords)
                .Take(pageSize);
            var result = new PagedResult<MakePost>
            {
                Data = resource.AsNoTracking().ToList(),
                TotalItems = context.MakePosts.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize

            };
            return View(result);
        }
        //Get AllResource detail acation method
        public ActionResult BlogDetails(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var product = context.MakePosts.Include(c => c.Category)
                .Where(c => c.MakepostStatus == "Enable" && c.IsApproved == true).FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.gallery = context.Galleries.Where(c => c.MakePostId == product.Id);
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
               
                Email = user.Email,
               
                FirstName = user.FirstName,
                LastName = user.LastName,

               
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
               
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;


                if (model.UserImage != null)
                {
                    var name = Path.Combine(webHostEnvironment.WebRootPath + "/images", Path.GetFileName(model.UserImage.FileName));
                    await model.UserImage.CopyToAsync(new FileStream(name, FileMode.Create));
                    user.UserImagePath = "images/" + model.UserImage.FileName;
                }

                if (model.UserImage == null)
                {
                    user.UserImagePath = "images/noimage.PNG";
                }

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        public IActionResult Search(string query, string departmentName, string subjectName)
        {
            var academicResources = context.AcademicResources.Include(c => c.Category).Include(d => d.Department)
                .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable");

            var departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(); // Fetch and store the departments

            var subjects = context.Subjects.Include(c => c.Department).Where(c => c.SubjectStatus == "Enable");

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();

                academicResources = academicResources.Where(item =>
                    item.Title.ToLower().Contains(query)
                );
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                departmentName = departmentName.ToLower();

                // Filter the stored departments instead of requerying the database
                departments = departments.Where(item =>
                    item.Name.ToLower().Contains(departmentName)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(subjectName))
            {
                subjectName = subjectName.ToLower();

                academicResources = academicResources.Where(item =>
                    item.Subject.Name.ToLower().Contains(subjectName)
                );
            }

            var searchVM = new SearchVM
            {
                AcademicResource = academicResources.ToList(),
                Departments = departments // Add departments to the ViewModel
            };

            return View("Search", searchVM);
        }


        [HttpPost]
        [ActionName("Search")]
        public IActionResult Searchs(string query, string departmentName, string subjectName)
        {
            var academicResources = context.AcademicResources.Include(c => c.Category).Include(d => d.Department)
                .Include(e => e.Subject).Where(c => c.AcademicResourceStatus == "Enable");

            var departments = context.Departments.Where(c => c.DepartmentStatus == "Enable").ToList(); // Fetch and store the departments

            var subjects = context.Subjects.Include(c => c.Department).Where(c => c.SubjectStatus == "Enable");

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();

                academicResources = academicResources.Where(item =>
                    item.Title.ToLower().Contains(query)
                );
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                departmentName = departmentName.ToLower();

                // Filter the stored departments instead of requerying the database
                departments = departments.Where(item =>
                    item.Name.ToLower().Contains(departmentName)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(subjectName))
            {
                subjectName = subjectName.ToLower();

                academicResources = academicResources.Where(item =>
                    item.Subject.Name.ToLower().Contains(subjectName)
                );
            }

            var searchVM = new SearchVM
            {
                AcademicResource = academicResources.ToList(),
                Departments = departments // Add departments to the ViewModel
            };

            return View("Search", searchVM);
        }




    }


}
