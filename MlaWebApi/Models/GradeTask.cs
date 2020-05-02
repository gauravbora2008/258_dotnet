using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class GradeTask
    {
        public int idTask { get; set; }
        public string subject_id { get; set; }
        public string instructor_id { get; set; }
        public string topic { get; set; }
        public string description { get; set; }
        public DateTime schedule_startTime { get; set; }
        public DateTime schedule_endTime { get; set; }
        public string isQuiz { get; set; } // y for yes and n for no
        public string repeatTask { get; set; } // smw or tt or d(daily)


        public string student_id { get; set; }
        public string instr_grade { get; set; } 
    }
}