using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public List<MakePost> Posts { get; set; }
        public List<AcademicResource> AcademicResources { get; set; }
    }
}

