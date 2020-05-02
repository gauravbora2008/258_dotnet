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
using System.Windows.Forms;
namespace MlaWebApi.Controllers
{
    public class EnrollStudentController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Subject> GetSubjectByStd(string idStudent)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select distinct(subject_id) from subject_roster where student_id = '" + idStudent + "' ", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("register");
            Sqlda.Fill(dsDatast);

            int count = dsDatast.Tables[0].Rows.Count;
            if (dsDatast.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dsDatast.Tables[0].Rows)
                {
                    string str = dsDatast.Tables[0].Rows[count - 1][0].ToString();
                    SqlCommand commsubjname = new SqlCommand("Select idSubject, title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType from subject where idSubject='" + str + "'", cnn);
                    Sqlda = new SqlDataAdapter(commsubjname);
                    DataSet dsDatasubj = new DataSet("subject name");
                    Sqlda.Fill(dsDatasubj);
                    yield return new Subject
                    {
                        idSubject = Convert.ToString(row["idSubject"]),
                        title = Convert.ToString(row["title"]),
                        description = Convert.ToString(row["description"]),
                        subjectTerm = Convert.ToString(row["subjectTerm"]),
                        videoEnabled = Convert.ToString(row["videoEnabled"]),
                        audioEnabled = Convert.ToString(row["audioEnabled"]),
                        startDate = Convert.ToDateTime(row["startDate"]),
                        endDate = Convert.ToDateTime(row["endDate"]),
                        idInstructor = Convert.ToString(row["idInstructor"]),
                        startTime = Convert.ToString(row["startTime"]),
                        endTime = Convert.ToString(row["endTime"]),
                        mailingAlias = Convert.ToString(row["mailingAlias"]),
                        duration = Int32.Parse(Convert.ToString(row["duration"])),
                        subjectType = Convert.ToString(row["subjectType"])
                    };
                    count--;
                }
            }
            else
            {
                yield return new Subject
                {
                    idSubject = null,
                    title = null,
                    description = null,
                    subjectTerm = null,
                    videoEnabled = null,
                    audioEnabled = null,
                    //startDate = null,
                    //endDate = null,
                    idInstructor = null,
                    startTime = null,
                    endTime = null,
                    mailingAlias = null,
                    //duration = null,
                    subjectType = null
                };

            }
        }
        public IEnumerable<Student> GetEnrollBySubject(string idSubject)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idStudent,firstName,lastName,emailId  from student,subject_roster where student.idStudent=subject_roster.student_id and subject_roster.subject_id= '" + idSubject + "' ", cnn);
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
        public HttpResponseMessage PostEnrollStudent(Subject_Roster subject_roster)  
        {
            DataSet dsSubjectRoster = new DataSet("Subject_Roster");
            DataSet dsGroupRoster = new DataSet("Group_Roster");
            DataSet dsInterestGroup = new DataSet("InterestGroup");
            DataSet dsSubject = new DataSet("subject");
            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
          
            try
            {
                //Retrieve the mailingAlias from the subject in order to find the groupId
                SqlCommand commn = new SqlCommand("select idInstructor from subject where idSubject='"
                     + subject_roster.subject_id + "'", cnn);
                SqlDataAdapter Sqldan = new SqlDataAdapter(commn);
                dsSubject = new DataSet("n");
                Sqldan.Fill(dsSubject);

                string instructor_id = dsSubject.Tables[0].Rows[0][0].ToString();
              
                    SqlCommand comm = new SqlCommand("Insert into subject_roster(student_id,subject_id,instructor_id) values('"
                     + subject_roster.student_id + "', '"
                     + subject_roster.subject_id + "', '"
                     + instructor_id + "')", cnn);
                    SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
                    Sqlda.Fill(dsSubjectRoster);

                    //Retrieve the mailingAlias from the subject in order to find the groupId
                    SqlCommand comm2 = new SqlCommand("select mailingAlias from subject where idSubject='"
                         + subject_roster.subject_id + "'", cnn);
                    SqlDataAdapter Sqlda2 = new SqlDataAdapter(comm2);
                    Sqlda2.Fill(dsSubject);
                    
                    string subject_mailingAlias = dsSubject.Tables[0].Rows[0][0].ToString();
                 
                    //Retrieve the groupId from the interest_group
                  /*  SqlCommand comm3 = new SqlCommand("select groupId,groupmailingalias from interest_group where groupMailingAlias='"
                         + subject_mailingAlias + "'", cnn);
                    Sqlda = new SqlDataAdapter(comm3);
                    Sqlda.Fill(dsInterestGroup);

                    string groupId = dsInterestGroup.Tables[0].Rows[0][0].ToString();
                    string groupMailID = dsInterestGroup.Tables[0].Rows[0][1].ToString();
                
                    //Retrieve the userId from the student table
                    SqlCommand comm4 = new SqlCommand("select userId from student where idStudent='"
                         + subject_roster.student_id +"' ", cnn);
                    Sqlda = new SqlDataAdapter(comm4);
                    DataSet dsStudent=new DataSet("student");
                    Sqlda.Fill(dsStudent);

                    string userId = dsInterestGroup.Tables[0].Rows[0][0].ToString();
                    MessageBox.Show(userId);
                    SqlCommand comm5 = new SqlCommand("Insert into group_roster(groupId,userId,mailing_alias) values("
                     + groupId + ", "
                     + userId + ", '"
                     + groupMailID + "')", cnn);
                    Sqlda = new SqlDataAdapter(comm5);
                    Sqlda.Fill(dsGroupRoster);
                */

                    SqlCommand comm6 = new SqlCommand("Select idTask from tasks where subject_id ='" + subject_roster.subject_id + "' and instructor_id = '" + instructor_id + "'", cnn);
                    SqlDataAdapter sqlceda5 = new SqlDataAdapter(comm6);
                    dsData = new DataSet();
                    sqlceda5.Fill(dsData);
                    
                    if (dsData.Tables[0].Rows.Count >= 0)
                    {
                        string[] idTasks = new String[dsData.Tables[0].Rows.Count];
                        for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                        {
                            idTasks[i] = dsData.Tables[0].Rows[i][0].ToString();
                        }


                        for (int j = 0; j < idTasks.Count(); j++)
                        {
                            SqlCommand comm7 = new SqlCommand("Insert into student_tasks(student_id, subject_id, tasks_id, instr_grade) values('"
                                                    + subject_roster.student_id
                                                    + "','" + subject_roster.subject_id
                                                    + "'," + idTasks[j]
                                                    + ",'" + ""
                                                    + "')", cnn);
                            SqlDataAdapter sqlada6 = new SqlDataAdapter(comm7);
                            dsData = new DataSet();
                            sqlada6.Fill(dsData);

                        }
                    }



                    var response = Request.CreateResponse<Subject_Roster>(System.Net.HttpStatusCode.Created, subject_roster);
                    cnn.Close();
                    return response;
            }                
            catch (Exception e)
            {
               
                var response = Request.CreateResponse<Subject_Roster>(System.Net.HttpStatusCode.BadRequest, subject_roster);
                cnn.Close();
                return response;
            }
        }
    }
  
}

