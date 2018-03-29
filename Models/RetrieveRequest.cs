using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynService_v3.Models
{
    public class RetrieveRequest
    {
        [Required]
        [Display(Name = "Entity")]
        public string Entity { get; set; }

        [Display(Name = "Fields")]
        public List<string> Fields { get; set; }

        [Display(Name = "Conditions")]
        public List<RetrieveCondition> Conditions { get; set; }

        [Display(Name = "Order")]
        public List<string> Order { get; set; }
        [Display(Name = "WhereOperator")]
        public string WhereOperator { get; set; }
        [Display(Name = "OrderDirection")]
        public string OrderDirection { get; set; }
    }

    public class RetrieveCondition
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public List<object> Values { get; set; }
    }
}
