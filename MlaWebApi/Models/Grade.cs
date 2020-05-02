using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Grade
    {
        public int idGrade { get; set; }
        public string selfGrade { get; set; }
        public string instuctorGrade { get; set; }
        public string subject_id { get; set; }
        public string overallGrade { get; set; }
        public string instructor_id { get; set; }
        public string student_id { get; set; }
        public int task_id { get; set; }
    }
}