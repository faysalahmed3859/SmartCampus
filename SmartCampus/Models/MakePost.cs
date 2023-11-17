using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Models
{
    public class MakePost
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public DateTime Date { get; set; }
        public string MakepostStatus { get; set; }
        //Navigation
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }



    }
}
