using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynService_v3.Models
{
    public class UpdateResponse
    {
        public UpdateRequest Request { get; set; }
        public Entity Result { get; set; }

        public Guid Id { get; set; }
        public List<Dictionary<string, string>> Errors { get; set; }
    }
}