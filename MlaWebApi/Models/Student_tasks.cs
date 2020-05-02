using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Student_tasks
    {
        public string student_id { get; set; }
        public string subject_id { get; set; }
        public int tasks_id { get; set; }
        public int grade_id { get; set; }
        public string instr_grade { get; set; } // y = yes and n=no
    }
}