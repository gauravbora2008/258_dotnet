using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Subject_Roster
    {
        public string subject_id { get; set; }
        public string student_id { get; set; }
        public string instructor_id { get; set; }
    }
}