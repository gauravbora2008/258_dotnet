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
using System.Security.Cryptography.X509Certificates;

namespace MlaWebApi.Controllers
{
    public class SocialController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<SN_User> GetAllUsers()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select username, password, publicKey, fullname, user_id  from sn_user", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("sn_user");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new SN_User
                {
                    username = Convert.ToString(row["username"]).Trim(),
                    password = Convert.ToString(row["password"]).Trim(),
                    publicKey = Convert.ToString(row["publicKey"]).Trim(),
                    user_id = Int32.Parse(Convert.ToString(row["user_id"]).Trim()),
                    fullname = Convert.ToString(row["fullname"]).Trim(),
                };
            }

        }

        public HttpResponseMessage RegisterNewUser(
            string username, 
            string password, 
            string publicKey, 
            string fullname,
            string groupKey)
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                String commandText = "insert into sn_user (username, password, publicKey, fullname) values ('"
                    + username + "','"
                    + password + "','"
                    + publicKey + "','"
                    + fullname + "');"
                    + "insert into sn_user (username, password, publicKey, fullname) values ('"
                    + username + "','"
                    + password + "','"
                    + publicKey + "','"
                    + fullname + "');";

                SqlCommand comm = new SqlCommand(commandText, cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.Found, "User added " + username);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, "Error! User could not be added");
                cnn.Close();
                return response;
            }

        }

        //public IEnumerable<Admin> GetAllAdmin()
        //{
        //    cnn = new SqlConnection(cfmgr);
        //    cnn.Open();

        //    SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin", cnn);
        //    SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
        //    DataSet dsDatast = new DataSet("admin");
        //    Sqlda.Fill(dsDatast);

        //    foreach (DataRow row in dsDatast.Tables[0].Rows)
        //    {
        //        yield return new Admin
        //        {
        //            idAdmin = Convert.ToString(row["idAdmin"]),
        //            firstName = Convert.ToString(row["firstName"]),
        //            lastName = Convert.ToString(row["lastName"]),
        //            userId = Int32.Parse(Convert.ToString(row["userId"])),
        //            telephone = Convert.ToString(row["telephone"]),
        //            address = Convert.ToString(row["address"]),
        //            aliasMailId = Convert.ToString(row["aliasMailId"]),
        //            emailId = Convert.ToString(row["emailId"]),
        //            skypeId = Convert.ToString(row["skypeId"])
        //        };
        //    }

        //}

        //public IEnumerable<Admin> GetAdminByUserName(string userName)
        //{

        //    cnn = new SqlConnection(cfmgr);
        //    cnn.Open();

        //    SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin where idAdmin = '" + userName + "'", cnn);
        //    SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

        //    DataSet dataSet = new DataSet("student");
        //    Sqlda.Fill(dataSet);

        //    foreach (DataRow row in dataSet.Tables[0].Rows)
        //    {
        //        yield return new Admin
        //        {
        //            idAdmin = Convert.ToString(row["idAdmin"]),
        //            firstName = Convert.ToString(row["firstName"]),
        //            lastName = Convert.ToString(row["lastName"]),
        //            userId = Int32.Parse(Convert.ToString(row["userId"])),
        //            telephone = Convert.ToString(row["telephone"]),
        //            address = Convert.ToString(row["address"]),
        //            aliasMailId = Convert.ToString(row["aliasMailId"]),
        //            emailId = Convert.ToString(row["emailId"]),
        //            skypeId = Convert.ToString(row["skypeId"])
        //        };
        //    }
        //}

        //public IEnumerable<Admin> GetAdminByUserId(int userId)
        //{

        //    cnn = new SqlConnection(cfmgr);
        //    cnn.Open();

        //    SqlCommand comm = new SqlCommand("Select idAdmin, firstName, lastName, userId, telephone, address, aliasMailId, emailId, skypeId  from admin where userId = " + userId + " ", cnn);
        //    SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

        //    DataSet dataSet = new DataSet("admin");
        //    Sqlda.Fill(dataSet);

        //    foreach (DataRow row in dataSet.Tables[0].Rows)
        //    {
        //        yield return new Admin
        //        {
        //            idAdmin = Convert.ToString(row["idAdmin"]),
        //            firstName = Convert.ToString(row["firstName"]),
        //            lastName = Convert.ToString(row["lastName"]),
        //            userId = Int32.Parse(Convert.ToString(row["userId"])),
        //            telephone = Convert.ToString(row["telephone"]),
        //            address = Convert.ToString(row["address"]),
        //            aliasMailId = Convert.ToString(row["aliasMailId"]),
        //            emailId = Convert.ToString(row["emailId"]),
        //            skypeId = Convert.ToString(row["skypeId"])
        //        };
        //    }
        //}


    }
}
