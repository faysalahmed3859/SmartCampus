using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Models
{
    public class AcademicResource
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string PdfPath { get; set; }
        public string ImagePath { get; set; }
        public bool IsApproved { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string AcademicResourceStatus { get; set; }
        //Navigation
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department  { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }


    }
}
