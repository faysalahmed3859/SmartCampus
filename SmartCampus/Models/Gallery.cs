using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartCampus.Models
{
    public class Gallery
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        //Navigation
        public Guid MakePostId { get; set; }
        public MakePost MakePost { get; set; }
      

    }
}
