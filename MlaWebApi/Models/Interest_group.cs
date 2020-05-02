using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Interest_group
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string groupDescription { get; set; }
        public string groupMailingAlias { get; set; }
    }
}