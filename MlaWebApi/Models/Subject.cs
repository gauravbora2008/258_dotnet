using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MlaWebApi.Models
{
    public class Subject
    {
        public string idSubject { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string subjectTerm { get; set; }
        public string videoEnabled { get; set; } //y=yes  n=no
        public string audioEnabled { get; set; } //y=yes  n=no
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string idInstructor { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string mailingAlias { get; set; }
        public int duration { get; set; }
        public string subjectType { get; set; }

    }
}