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
    public class DeEnrollStudentController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Student> GetDeEnrollBySubject(string idSubject)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

           // SqlCommand comm = new SqlCommand("Select idStudent,firstName,lastName,emailId  from student,subject_roster where student.idStudent!=subject_roster.student_id and subject_roster.subject_id= '" + idSubject + "' ", cnn);

            SqlCommand comm = new SqlCommand("Select idStudent,firstName,lastName,emailId  from student WHERE NOT EXISTS (SELECT subject_roster.student_id from subject_roster WHERE subject_roster.student_id=student.idStudent and   subject_roster.subject_id= '" + idSubject + "' )", cnn);
            
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("register");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {

                yield return new Student
                {
                    idStudent = Convert.ToString(row["idStudent"]),
                    firstName = Convert.ToString(row["firstName"]),
                    lastName = Convert.ToString(row["lastName"]),
                    emailId = Convert.ToString(row["emailId"])
                };
            }
        }

        public HttpResponseMessage PostDeEnrollStd(Subject_Roster subject_roster)
        {
            DataSet dsData = new DataSet("Subject_Roster");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm6 = new SqlCommand("DELETE FROM student_tasks where subject_id ='" + subject_roster.subject_id + "' and student_id ='" + subject_roster.student_id + "'", cnn);
                SqlDataAdapter Sqlda6 = new SqlDataAdapter(comm6);
                dsData = new DataSet();
                Sqlda6.Fill(dsData);

                SqlCommand comm5 = new SqlCommand("DELETE FROM subject_roster where subject_id='"+subject_roster.subject_id+"' and student_id ='" + subject_roster.student_id + "'", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.Found, null);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.BadRequest, null);
                cnn.Close();
                return response;
            }
        }
    }
}