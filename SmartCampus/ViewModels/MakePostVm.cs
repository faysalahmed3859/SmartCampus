using Microsoft.AspNetCore.Http;
using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class MakePostVm
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImagePath { get; set; }
        public bool IsApproved { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string MakepostStatus { get; set; }
        //Navigation
        public Guid CategoryId { get; set; }
       
        public List<IFormFile> Galleries { get; set; }
        public List<Gallery> GalleryImagesPath { get; set; }
    }
}
