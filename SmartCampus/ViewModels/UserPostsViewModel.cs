using SmartCampus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class UserPostsViewModel
    {
        public ApplicationUser TargetUser { get; set; }
        public List<MakePost> UserPosts { get; set; }
        public List<AcademicResource> AcademicResources { get; set; }

    }
}
