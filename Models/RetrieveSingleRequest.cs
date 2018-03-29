using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class RetrieveSingleRequest
    {
        [Required]
        [Display(Name = "Entity")]
        public string Entity { get; set; }

        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Fields")]
        public List<string> Fields { get; set; }
    }
}