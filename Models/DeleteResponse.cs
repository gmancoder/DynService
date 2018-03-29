using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class DeleteResponse
    {
        public DeleteRequest request { get; set; }
        public Boolean success { get; set; }
        public List<Dictionary<string, string>> Errors { get; set; }
    }
}