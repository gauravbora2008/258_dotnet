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
            string publicKeyString, 
            string fullname,
            string encryptedGroupKey)
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                String commandText = $"insert into sn_user (username, password, publicKey, fullname) " +
                                    $" values ('{username}', '{password}', '{publicKeyString}', '{fullname}');";

                SqlCommand comm = new SqlCommand(commandText, cnn);
                SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
                DataSet dsDatast = new DataSet("newUser");
                Sqlda.Fill(dsDatast);

                SqlCommand comm2 = new SqlCommand($"Select user_id from sn_user where username = '{username}'", cnn);
                SqlDataAdapter Sqlda2 = new SqlDataAdapter(comm2);
                dsData = new DataSet();
                Sqlda2.Fill(dsData);

                int userId = Int32.Parse(dsData.Tables[0].Rows[0]["user_id"].ToString());

                SqlCommand comm3 = new SqlCommand($"insert into sn_group (group_name, group_owner_id, member_id) " +
                                                $"values ('friends_group', '{userId}', '{userId}')", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                SqlCommand comm4 = new SqlCommand($"Select group_id from sn_group where group_name = 'friends_group' and group_owner_id = '{userId}'", cnn);
                SqlDataAdapter Sqlda4 = new SqlDataAdapter(comm4);
                dsData = new DataSet();
                Sqlda4.Fill(dsData);

                int groupId = Int32.Parse(dsData.Tables[0].Rows[0]["group_id"].ToString());

                SqlCommand comm5 = new SqlCommand($"insert into sn_group_key_table (owner_id, member_id, group_key, group_id, key_version, key_status) values ('{userId}', '{userId}', '{encryptedGroupKey}', '{groupId}', '1', '0')", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);                

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.Found, "User added id: " + userId + "friends group id : " + groupId);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, e.ToString() + "\n\n" + dsData);
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
