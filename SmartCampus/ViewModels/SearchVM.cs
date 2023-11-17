using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class SearchVM
    {
       
        public IEnumerable<AcademicResource> AcademicResource { get; set; }
        public IEnumerable<Department> Departments { get; set; }

    }
}
