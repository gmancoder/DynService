using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class RetrieveResponse
    {
        public RetrieveRequest request { get; set; }
        public EntityCollection Results { get; set; }
        public List<Dictionary<string, string>> Errors { get; set; }
    }
}