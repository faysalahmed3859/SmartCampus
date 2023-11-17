using SmartCampus.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class AcademicResourceVm
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public string ImagePath { get; set; }
        public string PdfPath { get; set; }
        public string FileUpload { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string AcademicResourceStatus { get; set; }
        //Navigation
        public Guid CategoryId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid DepartmentId { get; set; }
        
        public List<IFormFile> PdfCollections { get; set; }
        public List<PdfCollection> PdfCollectionsPath { get; set; }
        public List<IFormFile> ImageCollections { get; set; }
        public List<ImageCollection> ImageCollectionPath { get; set; }
    }
}
