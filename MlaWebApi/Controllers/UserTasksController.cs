using MlaWebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
//using System.Data.SqlServerCe;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
namespace MlaWebApi.Controllers
{
    public class UserTasksController : ApiController
    {

        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Tasks> GetTasksByUser(string userId,string userType)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            SqlCommand comm;
            comm = new SqlCommand("Select * from tasks,student_tasks where tasks.idTask=student_tasks.tasks_id and student_tasks.student_id ='" + userId + "'", cnn);
            
            if (userType != "student")
            {
               
                  comm = new SqlCommand("Select * from tasks where instructor_id ='" + userId + "'", cnn);
       
            }
            
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("tasks");
            Sqlda.Fill(dsDatast);


            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new Tasks
                {
                    idTask = Int16.Parse(Convert.ToString(row["idTask"])),
                    subject_id = Convert.ToString(row["subject_id"]),
                    instructor_id = Convert.ToString(row["instructor_id"]),
                    topic = Convert.ToString(row["topic"]),
                    description = Convert.ToString(row["description"]),
                    schedule_startTime = Convert.ToDateTime(row["schedule_startTime"]),
                    schedule_endTime = Convert.ToDateTime(row["schedule_endTime"]),
                    isQuiz = Convert.ToString(row["isQuiz"]),
                    repeatTask = Convert.ToString(row["repeatTask"])
                };
            }
            cnn.Close();

        }

    }
}
