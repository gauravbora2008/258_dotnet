using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Group_roster
    {
        public class group_roster
        {
            public int groupSeqId { get; set; }
            public int groupId { get; set; }
            public int userId { get; set; }
            public string mailing_alias { get; set; }
        }
    }
}