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
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http.Headers;

namespace MlaWebApi.Controllers
{
    public class SocialController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<SN_UserLogin> LoginAuth(string username, string password)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand($"Select user_id, username, fullname, publicKey from sn_user where username = '{username}' and password = '{password}'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet loginDataSet = new DataSet("userLogin");
            Sqlda.Fill(loginDataSet);

            foreach (DataRow row in loginDataSet.Tables[0].Rows)
            {
                yield return new SN_UserLogin
                {
                    user_id = Int16.Parse(Convert.ToString(row["user_id"])),
                    fullname = Convert.ToString(row["fullname"]).Trim(),
                    publicKey = Convert.ToString(row["publicKey"]).Trim(),
                    username = Convert.ToString(row["username"]).Trim()
                };
            }

        }

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

        public IEnumerable<GetGroupsByMemberIdModel> GetGroupsByMemberId(int memberId)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand($"SELECT T1.group_id, group_name, group_key FROM sn_group T1 INNER JOIN sn_group_key_table T2 ON T1.group_id = T2.group_id where T1.member_id = {memberId}", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetGroupsByMemberIdModel
                {
                    group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim()
                };
            }
        }

        public IEnumerable<GetGroupsByMemberIdModel> GetGroupsByNotAMemberId(int notJoinedMemberId)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand(
                $"SELECT T1.group_id, T1.group_owner_id, T3.fullname, group_name, group_key " +
                $"FROM sn_group T1 " +
                $"INNER JOIN sn_group_key_table T2 ON T1.group_id = T2.group_id " +
                $"INNER JOIN sn_user T3 ON T1.group_owner_id = T3.user_id " +
                $"where T1.member_id != {notJoinedMemberId}", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups2");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetGroupsByMemberIdModel
                {
                    group_owner_id = Convert.ToString(row["group_owner_id"]).Trim(),
                    group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    owner_fullname = Convert.ToString(row["fullname"]).Trim()
                };
            }
        }

        public IEnumerable<GetPostsByMemberIdModel> GetPosts(int userId2)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand(
                $"SELECT T1.author_id, T2.fullname, T1.group_id, T3.group_name, T4.group_key, " +
                $"T1.post_data, T1.post_key, T1.timestamp " +
                $"FROM sn_posts T1 " +
                $"INNER JOIN sn_user T2 ON T1.author_id = T2.user_id " +
                $"INNER JOIN sn_group T3 ON T1.group_id = T3.group_id " +
                $"INNER JOIN sn_group_key_table T4 ON T1.group_id = T4.group_id " +
                $"where T1.author_id = '{userId2}'", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetPostsByMemberIdModel
                {

                    authod_id = Convert.ToString(row["author_id"]).Trim(),
                    fullname = Convert.ToString(row["fullname"]).Trim(),
                    group_id = Convert.ToString(row["group_id"]).Trim(),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    post_data = Convert.ToString(row["post_data"]).Trim(),
                    post_key = Convert.ToString(row["post_key"]).Trim(),
                    timestamp = Convert.ToString(row["timestamp"]).Trim()
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
                if (e.ToString().Contains("Violation of PRIMARY KEY"))
                {
                    var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, "User Already Exists " + e.ToString());
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, e.ToString());
                    cnn.Close();
                    return response;
                }


            }

        }

        public HttpResponseMessage CreateNewGroup(
           string owner_id,
           string group_name,
           string encryptedGroupKey)
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                SqlCommand comm3 = new SqlCommand($"insert into sn_group (group_name, group_owner_id, member_id) " +
                                                $"values ('{group_name}', '{owner_id}', '{owner_id}')", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                SqlCommand comm4 = new SqlCommand($"Select group_id from sn_group where group_name = '{group_name}' and group_owner_id = '{owner_id}'", cnn);
                SqlDataAdapter Sqlda4 = new SqlDataAdapter(comm4);
                dsData = new DataSet();
                Sqlda4.Fill(dsData);

                int groupId = Int32.Parse(dsData.Tables[0].Rows[0]["group_id"].ToString());

                SqlCommand comm5 = new SqlCommand($"insert into sn_group_key_table (owner_id, member_id, group_key, group_id, key_version, key_status) values ('{owner_id}', '{owner_id}', '{encryptedGroupKey}', '{groupId}', '1', '0')", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.Found, "Group created id: " + groupId);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, e.ToString());
                cnn.Close();
                return response;
            }

        }

        public HttpResponseMessage CreateNewPost(
           string author_id,
           string group_id,
           string post_key,
           string post_data,
           string timestamp
            )
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                SqlCommand comm3 = new SqlCommand($"insert into sn_posts (author_id, group_id, post_key, post_data, timestamp) " +
                                                $"values ('{author_id}', '{group_id}', '{post_key}', '{post_data}', '{timestamp}')", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "Post Added !");

                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, e.ToString());
                cnn.Close();
                return response;
            }

        }

        public HttpResponseMessage CreateNewAddRequest(
          string user_id,
          string group_id,
          string group_owner_id
           )
        {

            DataSet dsData = new DataSet("addRequest");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                SqlCommand comm3 = new SqlCommand(
                    $"insert into sn_add_requests (user_id, group_id, group_owner_id) " +
                    $"values ('{user_id}', '{group_id}', '{group_owner_id}')", cnn);

                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "Request Sent !");

                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, e.ToString());
                cnn.Close();
                return response;
            }

        }

        public IEnumerable<GetPendingAddRequestsModel> GetPendingAddRequests(string user_id)
        {


            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm3 = new SqlCommand(
                $"SELECT T1.user_id, T3.publicKey, T1.group_id, T2.group_key, T4.group_name, T1.group_owner_id " +
                $"FROM sn_add_requests T1 " +
                $"INNER JOIN sn_user T3 ON T1.user_id = T3.user_id " +
                $"INNER JOIN sn_group_key_table T2 ON T1.group_id = T2.group_id " +
                $"INNER JOIN sn_group T4 ON T1.group_id = T4.group_id " +
                $"where T1.group_owner_id = '{user_id}'", cnn);

            SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
            DataSet psDataSet = new DataSet("pendingAddRequests");
            Sqlda3.Fill(psDataSet);

            foreach (DataRow row in psDataSet.Tables[0].Rows)
            {
                yield return new GetPendingAddRequestsModel
                {

                    user_id = Convert.ToString(row["user_id"]).Trim(),
                    public_key = Convert.ToString(row["publicKey"]).Trim(),
                    group_id = Convert.ToString(row["group_id"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_owner_id = Convert.ToString(row["group_owner_id"]).Trim(),

                };
            }





        }
    }
}
