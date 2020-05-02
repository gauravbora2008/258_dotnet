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
    public class StudentController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Student> GetAllStudent()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idStudent, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from student", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("student");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new Student
                {
                    idStudent = Convert.ToString(row["idStudent"]),
                    firstName = Convert.ToString(row["firstName"]),
                    lastName = Convert.ToString(row["lastName"]),
                    userId = Int32.Parse(Convert.ToString(row["userId"])),
                    telephone = Convert.ToString(row["telephone"]),
                    address = Convert.ToString(row["address"]),
                    aliasMailId = Convert.ToString(row["aliasMailId"]),
                    emailId = Convert.ToString(row["emailId"]),
                    skypeId = Convert.ToString(row["skypeId"])
                };
            }

        }

        public IEnumerable<Student> GetStudentByUserName(string userName)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idStudent, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from student where idStudent = '" + userName + "'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("student");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Student
                {
                    idStudent = Convert.ToString(row["idStudent"]),
                    firstName = Convert.ToString(row["firstName"]),
                    lastName = Convert.ToString(row["lastName"]),
                    userId = Int32.Parse(Convert.ToString(row["userId"])),
                    telephone = Convert.ToString(row["telephone"]),
                    address = Convert.ToString(row["address"]),
                    aliasMailId = Convert.ToString(row["aliasMailId"]),
                    emailId = Convert.ToString(row["emailId"]),
                    skypeId = Convert.ToString(row["skypeId"])
                };
            }
        }

        public IEnumerable<Student> GetStudentByUserId(int userId)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idStudent, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from student where userId = " + userId + " ", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("student");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Student
                {
                    idStudent = Convert.ToString(row["idStudent"]),
                    firstName = Convert.ToString(row["firstName"]),
                    lastName = Convert.ToString(row["lastName"]),
                    userId = Int32.Parse(Convert.ToString(row["userId"])),
                    telephone = Convert.ToString(row["telephone"]),
                    address = Convert.ToString(row["address"]),
                    aliasMailId = Convert.ToString(row["aliasMailId"]),
                    emailId = Convert.ToString(row["emailId"]),
                    skypeId = Convert.ToString(row["skypeId"])
                };
            }
        }

        public HttpResponseMessage PostStudentUpdate(Student student)
        {

            DataSet dsData = new DataSet("student");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm = new SqlCommand("Update student set firstName = '" + student.firstName + "'" + " , " + " lastName = '" + student.lastName + "'" + " , " + " telephone = '" + student.telephone + "'" + " , " + " address = '" + student.address + "'" + " , " + " aliasMailId = '" + student.aliasMailId + "'" + " , " + " emailId = '" + student.emailId + "'" + " , " + " skypeId = '" + student.skypeId + "'" + " where idStudent = '" + student.idStudent + "'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.Found, student);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.BadRequest, student);
                cnn.Close();
                return response;
            }

        }

    }
}
