using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MlaWebApi.Models;
//using System.Data.SqlServerCe;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace MlaWebApi.Controllers
{
    public class InstructorController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Instructor> GetAllInstructor()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idInstructor, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from instructor", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("instructor");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new Instructor
                {
                    idInstructor = Convert.ToString(row["idInstructor"]),
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

        public IEnumerable<Instructor> GetInstructorByUserName(string userName)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idInstructor, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from instructor where idInstructor = '" + userName + "'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("instructor");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Instructor
                {
                    idInstructor = Convert.ToString(row["idInstructor"]),
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

        public IEnumerable<Instructor> GetInstructorByUserId(int userId)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idInstructor, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from instructor where userId = " + userId +" ", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("instructor");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Instructor
                {
                    idInstructor = Convert.ToString(row["idInstructor"]),
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

        public HttpResponseMessage PostInstructorUpdate(Instructor instructor)
        {

            DataSet dsData = new DataSet("instructor");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm = new SqlCommand("Update instructor set firstName = '" + instructor.firstName + "'" + " , " + " lastName = '" + instructor.lastName + "'" + " , " + " telephone = '" + instructor.telephone + "'" + " , " + " address = '" + instructor.address + "'" + " , " + " aliasMailId = '" + instructor.aliasMailId + "'" + " , " + " emailId = '" + instructor.emailId + "'" + " , " + " skypeId = '" + instructor.skypeId + "'" + " where idInstructor = '" + instructor.idInstructor + "'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                var response = Request.CreateResponse<Instructor>(System.Net.HttpStatusCode.Found, instructor);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Instructor>(System.Net.HttpStatusCode.BadRequest, instructor);
                cnn.Close();
                return response;
            }

        }

    }
}
