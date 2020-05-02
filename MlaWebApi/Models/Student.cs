using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Student
    {
        public string idStudent { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string telephone { get; set; }
        public string address { get; set; }
        public int userId { get; set; }
        public string aliasMailId { get; set; }
        public string skypeId { get; set; }
    }
}