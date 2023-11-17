using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class IndexVM
    {
        public List<Banner> Banners { get; set; } 
        public List<AcademicResource> AcademicResources { get; set; }
       
    }
}
