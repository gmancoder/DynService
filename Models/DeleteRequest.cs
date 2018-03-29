using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class DeleteRequest
    {
        public string Entity { get; set; }
        public Guid Id { get; set; }
    }
}