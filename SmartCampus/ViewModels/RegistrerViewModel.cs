using SmartCampus.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.ViewModels
{
    public class RegistrerViewModel
    {
        
        [Required(ErrorMessage = "Please Enter  Email")]
        [EmailAddress]
        [Remote(action: "IsEmailInUse",controller:"Account")]
        [ValidEmailDomain(allowedDomain: "stamford.university",
            ErrorMessage = "Email domain must be stamford.university")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter  Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please Enter  Comfirm Password")]
        [Compare("Password", ErrorMessage = "Comfirm Password can't matched!")]
        public string ComfirmPassword { get; set; }
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Please enter a valid name.")]
        [Required(ErrorMessage = "Please Enter  Name")]
        public string FirstName { get; set; }
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Please enter a valid name.")]
        public string LastName { get; set; }

    }
}
