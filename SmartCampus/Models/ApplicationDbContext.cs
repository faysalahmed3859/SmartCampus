
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartCampus.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<MakePost> MakePosts { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<AcademicResource> AcademicResources { get; set; }
        public virtual DbSet<PdfCollection> PdfCollections { get; set; }
        public virtual DbSet<ImageCollection> ImageCollections { get; set; }


    }
}

