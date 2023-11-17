using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        [Required]        
        public string Name { get; set; }
        public string SubjectStatus { get; set; }
         //Navigation
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
