using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Models
{
    public class PdfCollection
    {
        public Guid Id { get; set; }
        public string PdfPath { get; set; }
        public string Status { get; set; }
        //Navigation
       
        public Guid AcademicResourceId { get; set; }
        public AcademicResource AcademicResource { get; set; }

    }
}
