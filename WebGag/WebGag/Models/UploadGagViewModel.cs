using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebGag.Models
{
    public class UploadGagViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Title { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string Description { get; set; }
        [Required]
        [FileExtensions(Extensions = "jpg;gif;png;mpg;mp4", ErrorMessage = "not allowed media type")]
        [Display(Description = "Content")]
        public HttpPostedFileBase Media { get; set; }
    }
}