using MlaWebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Windows.Forms;
using System.Collections;
namespace MlaWebApi.Controllers
{
    public class TasksController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Tasks> GetAllTasks()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select * from tasks", cnn);
            SqlDataAdapter sqlceda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("tasks");
            sqlceda.Fill(dsDatast);

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

        }
        public IEnumerable<Tasks> GetTasksBySubject(string subjectId)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select * from tasks where subject_id ='" + subjectId + "'", cnn);
            SqlDataAdapter sqlceda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("tasks");
            sqlceda.Fill(dsDatast);


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
        public IEnumerable<GradeTask> GetTasksByStudent(string studentId, string subject)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm2 = new SqlCommand("Select * from tasks,student_tasks where tasks.idTask=student_tasks.tasks_id and student_tasks.student_id ='" + studentId + "' and student_tasks.subject_id ='" + subject + "'", cnn);
            SqlDataAdapter sqlceda2 = new SqlDataAdapter(comm2);
            DataSet dsDatast = new DataSet("tasks");
            try
            {
                sqlceda2.Fill(dsDatast);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GradeTask
                {
                    idTask = Int16.Parse(Convert.ToString(row["idTask"])),
                    subject_id = Convert.ToString(row["subject_id"]),
                    instructor_id = Convert.ToString(row["instructor_id"]),
                    topic = Convert.ToString(row["topic"]),
                    description = Convert.ToString(row["description"]),
                    schedule_startTime = Convert.ToDateTime(row["schedule_startTime"]),
                    schedule_endTime = Convert.ToDateTime(row["schedule_endTime"]),
                    isQuiz = Convert.ToString(row["isQuiz"]),
                    repeatTask = Convert.ToString(row["repeatTask"]),
                    instr_grade = Convert.ToString(row["instr_grade"]),
                    student_id = Convert.ToString(row["student_id"])
                };
            }
            cnn.Close();
        }
        public IEnumerable<GradeTask> GetStudentByTask(string task, string subjectid)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm2 = new SqlCommand("Select * from tasks,student_tasks where tasks.idTask=student_tasks.tasks_id and student_tasks.tasks_id ='" + task + "' and student_tasks.subject_id ='" + subjectid + "'", cnn);
            SqlDataAdapter sqlceda2 = new SqlDataAdapter(comm2);
            DataSet dsDatast = new DataSet("tasks");
            try
            {
                sqlceda2.Fill(dsDatast);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GradeTask
                {
                    idTask = Int16.Parse(Convert.ToString(row["idTask"])),
                    subject_id = Convert.ToString(row["subject_id"]),
                    instructor_id = Convert.ToString(row["instructor_id"]),
                    topic = Convert.ToString(row["topic"]),
                    description = Convert.ToString(row["description"]),
                    schedule_startTime = Convert.ToDateTime(row["schedule_startTime"]),
                    schedule_endTime = Convert.ToDateTime(row["schedule_endTime"]),
                    isQuiz = Convert.ToString(row["isQuiz"]),
                    repeatTask = Convert.ToString(row["repeatTask"]),
                    instr_grade = Convert.ToString(row["instr_grade"]),
                    student_id = Convert.ToString(row["student_id"])
                };
            }
            cnn.Close();
        }
        public HttpResponseMessage PostGradeUpdate(string task_id, string student_id, string grade)
        {

            DataSet dsData = new DataSet("task");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();


            try
            {
                SqlCommand comm = new SqlCommand("Update student_tasks set instr_grade ='" + grade + "'" + " where tasks_id = '" + task_id + "'" + " and student_id = '" + student_id + "'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

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
        public HttpResponseMessage PostTaskUpdate(string idTask, string topic, string description)
        {

            DataSet dsData = new DataSet("task");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();


            try
            {
                SqlCommand comm = new SqlCommand("Update tasks set topic ='" + topic + "'" + " ,description = '" + description + "'" + " where idTask = '" + idTask + "'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

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
        public HttpResponseMessage PostTask(Tasks tasks)
        {
           // MessageBox.Show(tasks.ToString());
           // MessageBox.Show(tasks.repeatTask);
            //MessageBox.Show(tasks.subject_id);
            char[] text1 = tasks.repeatTask.ToCharArray();
            ArrayList array = new ArrayList();
            foreach (char s in text1)
            {
                if (s == 'm')
                {
                    array.Add("Monday");
                }
                else if (s == 'w')
                {
                    array.Add("Wednesday");
                }
                if (s == 'f')
                {
                    array.Add("Friday");
                }
                if (s == 't')
                {
                    array.Add("Tuesday");
                }
                if (s == 'r')
                {
                    array.Add("Thursday");
                }
                if (s == 's')
                {
                    array.Add("Saturday");
                }
                if (s == 'u')
                {
                    array.Add("Sunday");
                }

            }
            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            String idInstructor = "";
            String[] idStudent;

            try
            {

                SqlCommand comm = new SqlCommand("Select idTask from tasks where subject_id ='" + tasks.subject_id + "' ", cnn);
                SqlDataAdapter sqlceda = new SqlDataAdapter(comm);

                sqlceda.Fill(dsData);

                if (dsData.Tables[0].Rows.Count <= 0)
                {
                    SqlCommand comm2 = new SqlCommand("Select idInstructor from subject where idSubject ='" + tasks.subject_id + "' ", cnn);
                    SqlDataAdapter sqlceda2 = new SqlDataAdapter(comm2);
                    dsData = new DataSet();
                    sqlceda2.Fill(dsData);
                    idInstructor = dsData.Tables[0].Rows[0][0].ToString();
                    SqlCommand comm3 = new SqlCommand("Select student_id from subject_roster where subject_id ='" + tasks.subject_id + "' ", cnn);
                    SqlDataAdapter sqlceda3 = new SqlDataAdapter(comm3);
                    dsData = new DataSet();
                    sqlceda3.Fill(dsData);
                    idStudent = new String[dsData.Tables[0].Rows.Count];
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        idStudent[i] = dsData.Tables[0].Rows[i][0].ToString();
                    }

                    DateTime dateStart = tasks.schedule_startTime;

                    DateTime dateStart2 = dateStart;
                    DateTime dateEnd = tasks.schedule_endTime;
                    /*   if (tasks.repeatTask == "mwf")
                       {
                           while (dateStart2 < dateEnd)
                           {
                               DayOfWeek dayofweek = dateStart2.DayOfWeek;
                               if (dayofweek == DayOfWeek.Monday || dayofweek == DayOfWeek.Wednesday || dayofweek == DayOfWeek.Friday)
                               {
                                   SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                   
                                                       + tasks.subject_id
                                                       + "','" + idInstructor
                                                       + "','" + tasks.topic
                                                       + "','" + ""
                                                       + "','" + dateStart2
                                                       + "','" + dateStart2.ToShortDateString()+" "+dateEnd.ToShortTimeString()
                                                       + "','" + tasks.isQuiz
                                                       + "','" + tasks.repeatTask
                                                       + "')", cnn);
                                   SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                                   sqlada4.Fill(dsData);

                               }
                               dateStart2 = dateStart2.AddDays(1);
                           }

                       }
                       else if (tasks.repeatTask == "weekly")
                       {
                           DayOfWeek weeklyDay = dateStart2.DayOfWeek;
                           while (dateStart2 < dateEnd)
                           {
                               DayOfWeek dayofweek = dateStart2.DayOfWeek;
                               if (dayofweek == weeklyDay)
                               {
                                   SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                       + tasks.subject_id
                                                       + "','" + idInstructor
                                                       + "','" + tasks.topic
                                                       + "','" + ""
                                                       + "','" + dateStart2
                                                       + "','" + dateStart2.ToShortDateString() + " " + dateEnd.ToShortTimeString()
                                                       + "','" + tasks.isQuiz
                                                       + "','" + tasks.repeatTask
                                                       + "')", cnn);
                                   SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                                   sqlada4.Fill(dsData);
                               }
                               dateStart2 = dateStart2.AddDays(1);
                           }
                       }
                       else if (tasks.repeatTask == "tr")
                       {
                           while (dateStart2 < dateEnd)
                           {
                               DayOfWeek dayofweek = dateStart2.DayOfWeek;
                               MessageBox.Show(dayofweek.ToString());
                               if (dayofweek == DayOfWeek.Tuesday || dayofweek == DayOfWeek.Thursday)
                               {
                                   SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                       + tasks.subject_id
                                                       + "','" + idInstructor
                                                       + "','" + tasks.topic
                                                       + "','" + ""
                                                       + "','" + dateStart2
                                                       + "','" + dateStart2.ToShortDateString() + " " + dateEnd.ToShortTimeString()
                                                       + "','" + tasks.isQuiz
                                                       + "','" + tasks.repeatTask
                                                       + "')", cnn);
                                   SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                                   sqlada4.Fill(dsData);
                               }
                               dateStart2 = dateStart2.AddDays(1);
                           }
                       }
                       else if (tasks.repeatTask == "su")
                       {
                           while (dateStart2 < dateEnd)
                           {
                               DayOfWeek dayofweek = dateStart2.DayOfWeek;
                               MessageBox.Show(dayofweek.ToString());
                               if (array.Contains(dayofweek.ToString()))
                               {
                                   SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                       + tasks.subject_id
                                                       + "','" + idInstructor
                                                       + "','" + tasks.topic
                                                       + "','" + ""
                                                       + "','" + dateStart2
                                                       + "','" + dateStart2.ToShortDateString() + " " + dateEnd.ToShortTimeString()
                                                       + "','" + tasks.isQuiz
                                                       + "','" + tasks.repeatTask
                                                       + "')", cnn);
                                   SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                                   sqlada4.Fill(dsData);
                               }
                               dateStart2 = dateStart2.AddDays(1);
                           }
                       }
                       else if (tasks.repeatTask == "daily")
                       {
                           while (dateStart2 < dateEnd)
                           {
                               DayOfWeek dayofweek = dateStart2.DayOfWeek;
                               SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                   + tasks.subject_id
                                                   + "','" + idInstructor
                                                   + "','" + tasks.topic
                                                   + "','" + ""
                                                   + "','" + dateStart2
                                                   + "','" + dateStart2.ToShortDateString() + " " + dateEnd.ToShortTimeString()
                                                   + "','" + tasks.isQuiz
                                                   + "','" + tasks.repeatTask
                                                   + "')", cnn);
                               SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                               sqlada4.Fill(dsData);
            
                               dateStart2 = dateStart2.AddDays(1);
                           }
                       }*/
                    while (dateStart2 < dateEnd)
                    {
                        DayOfWeek dayofweek = dateStart2.DayOfWeek;
                        //   MessageBox.Show(dayofweek.ToString());
                        if (array.Contains(dayofweek.ToString()))
                        {
                            SqlCommand comm4 = new SqlCommand("Insert into tasks(subject_id,instructor_id,topic,description ,schedule_startTime ,schedule_endTime ,isQuiz ,repeatTask) values('"
                                                + tasks.subject_id
                                                + "','" + idInstructor
                                                + "','" + tasks.topic
                                                + "','" + ""
                                                + "','" + dateStart2
                                                + "','" + dateStart2.ToShortDateString() + " " + dateEnd.ToShortTimeString()
                                                + "','" + tasks.isQuiz
                                                + "','" + tasks.repeatTask
                                                + "')", cnn);
                            SqlDataAdapter sqlada4 = new SqlDataAdapter(comm4);
                            sqlada4.Fill(dsData);
                        }
                        dateStart2 = dateStart2.AddDays(1);
                    }

                    SqlCommand comm5 = new SqlCommand("Select idTask from tasks where subject_id ='" + tasks.subject_id + "' and instructor_id = '" + idInstructor + "'", cnn);
                    SqlDataAdapter sqlceda5 = new SqlDataAdapter(comm5);
                    dsData = new DataSet();
                    sqlceda5.Fill(dsData);
                    string[] idTasks = new String[dsData.Tables[0].Rows.Count];
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        idTasks[i] = dsData.Tables[0].Rows[i][0].ToString();
                    }

                    for (int i = 0; i < idStudent.Count(); i++)
                    {
                        for (int j = 0; j < idTasks.Count(); j++)
                        {
                            SqlCommand comm6 = new SqlCommand("Insert into student_tasks(student_id, subject_id, tasks_id, instr_grade) values('"
                                                    + idStudent[i]
                                                    + "','" + tasks.subject_id
                                                    + "'," + idTasks[j]
                                                    + ",'" + ""
                                                    + "')", cnn);
                            SqlDataAdapter sqlada6 = new SqlDataAdapter(comm6);
                            dsData = new DataSet();
                            sqlada6.Fill(dsData);

                        }
                    }
                    var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.Created, null);
                    cnn.Close();
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.BadRequest, null);
                    cnn.Close();
                    return response;
                }
            }
            catch (SqlException e)
            {
                //MessageBox.Show(e.ToString());
                var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.BadRequest, tasks);
                cnn.Close();
                return response;
            }
        }

        public HttpResponseMessage DeleteTask(string subject_id, string instructor_id)
        {

            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            try
            {
                SqlCommand comm5 = new SqlCommand("DELETE FROM student_tasks where subject_id ='" + subject_id + "'", cnn);
                SqlDataAdapter sqlceda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                sqlceda5.Fill(dsData);

                SqlCommand comm6 = new SqlCommand("DELETE FROM tasks where subject_id ='" + subject_id + "'", cnn);
                SqlDataAdapter sqlceda6 = new SqlDataAdapter(comm6);
                dsData = new DataSet();
                sqlceda6.Fill(dsData);

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


/*
  SqlCommand comm = new SqlCommand("Select idTask, subject_id, instructor_id, topic, description, schedule_startTime, schedule_endTime, isQuiz from tasks", cnn);
                SqlDataAdapter sqlceda = new SqlDataAdapter(comm);

                sqlceda.Fill(dsData);

                foreach (DataRow row in dsData.Tables[0].Rows)
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
                    };
                }*/