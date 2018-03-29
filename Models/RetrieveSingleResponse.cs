using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class RetrieveSingleResponse
    {
        public RetrieveSingleRequest request { get; set; }
        public Entity Result { get; set; }
        public List<Dictionary<string, string>> Errors { get; set; }
    }
}