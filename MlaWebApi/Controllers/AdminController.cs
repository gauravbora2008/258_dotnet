using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MlaWebApi.Models;
using System.Configuration;
//using System.Data.SqlServerCe;
using System.Data;
using System.Data.SqlClient;

namespace MlaWebApi.Controllers
{
    public class AdminController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Admin> GetAllAdmin()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("admin");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new Admin
                {
                    idAdmin = Convert.ToString(row["idAdmin"]),
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

        public IEnumerable<Admin> GetAdminByUserName(string userName)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin where idAdmin = '" + userName + "'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("student");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Admin
                {
                    idAdmin = Convert.ToString(row["idAdmin"]),
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

        public IEnumerable<Admin> GetAdminByUserId(int userId)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin where userId = " + userId + " ", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("admin");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Admin
                {
                    idAdmin = Convert.ToString(row["idAdmin"]),
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

        public HttpResponseMessage PostAdminUpdate(Admin admin)
        {

            DataSet dsData = new DataSet("admin");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm = new SqlCommand("Update admin set firstName = '" + admin.firstName + "'" + " , " + " lastName = '" + admin.lastName + "'" + " , " + " telephone = '" + admin.telephone + "'" + " , " + " address = '" + admin.address + "'" + " , " + " aliasMailId = '" + admin.aliasMailId + "'" + " , " + " emailId = '" + admin.emailId + "'" +" , " + " skypeId = '" + admin.skypeId + "'" + " where idAdmin = '" + admin.idAdmin + "'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                var response = Request.CreateResponse<Admin>(System.Net.HttpStatusCode.Found, admin);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Admin>(System.Net.HttpStatusCode.BadRequest, admin);
                cnn.Close();
                return response;
            }

        }
    }
}
