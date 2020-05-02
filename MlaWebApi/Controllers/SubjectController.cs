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
    public class SubjectController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Subject> GetAllSubject()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idSubject, title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType  from subject", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("subject");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
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
            }
        }

        public IEnumerable<Subject>  GetSubjectByInstructor(string idInstructor)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idSubject, title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType  from subject where idInstructor = '" + idInstructor+"'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("subject");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
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
            }
        }
        public IEnumerable<Subject> GetSubjectByStudent(string idStudent)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
 
            SqlCommand comm = new SqlCommand("Select idSubject, title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType  from subject where (idSubject in "+" (SELECT  subject_id from subject_roster where student_id = '" + idStudent + "'))",cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("subject");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
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
            }
        }

        public IEnumerable<Subject> GetSubjectByDate(DateTime startDate,DateTime endDate)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idSubject, title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType  from subject where startDate >= '" + startDate.ToString() + "'" + " and endDate <= '" + endDate.ToString()+"'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("subject");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
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
            }
        }

        public HttpResponseMessage PostSubject(Subject subject)
        {

            DataSet dsData = new DataSet("subject");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                SqlCommand comm = new SqlCommand("Insert into subject( idSubject,title, description, subjectTerm, videoEnabled, audioEnabled, startDate, endDate,idInstructor, startTime, endTime, mailingAlias, duration, subjectType ) values('"
                     + subject.idSubject
                    + "','" +subject.title
                    + "','" + subject.description
                    + "','" + subject.subjectTerm
                      + "','" + subject.videoEnabled
                    + "','" + subject.audioEnabled
                      + "','" + subject.startDate.ToString()
                    + "','" + subject.endDate.ToString()
                      + "','" + subject.idInstructor
                    + "','" + subject.startTime
                      + "','" + subject.endTime
                    + "','" + subject.mailingAlias
                      + "','" + subject.duration
                    + "','" + subject.subjectType
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                /*
                SqlCommand commInterestGroup = new SqlCommand("Insert into interest_group (groupName,groupDescription" +
                 ",groupMailingAlias) values('"
                 + subject.title + "', '"
                 + subject.description + "', '"
                 + subject.mailingAlias + "')", cnn);
                SqlDataAdapter sqlda = new SqlDataAdapter(commInterestGroup);
                DataSet ds = new DataSet("interest_group");
                sqlda.Fill(ds);*/

                var response = Request.CreateResponse<Subject>(System.Net.HttpStatusCode.Created, subject);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Subject>(System.Net.HttpStatusCode.BadRequest, subject);
                cnn.Close();
                return response;
            }

        }


        public IEnumerable<Subject> GetAllSubjectWithTask(string flag)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            SqlCommand comm = null;
            if (flag == "true")
            {
                comm = new SqlCommand("Select distinct subject.*  from subject,tasks where idSubject=subject_id", cnn);
            }
            else if (flag == "false")
            {
                comm = new SqlCommand("SELECT distinct subject.* from subject where idSubject NOT IN (select distinct subject_id from tasks)", cnn);
            }
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("subject");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
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
            }
        }

    }


}
