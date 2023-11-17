using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter  Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter  Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 
        [Display(Name = "Remember Me")]
        public bool IsRemember { get; set; }
    }
}
