using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class UpdateRequest
    {
        [Required]
        [Display(Name = "Entity")]
        public string Entity { get; set; }

        [Required]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Data")]
        public Dictionary<string, object> Data { get; set; }
    }
}