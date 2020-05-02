using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Message_table
    {
        public int msgId { get; set; }
        public string fromList { get; set; }
        public string toList { get; set; }
        public string msgSubject { get; set; }
        public string msgBody { get; set; }
        public string creationDate { get; set; }
    }
}