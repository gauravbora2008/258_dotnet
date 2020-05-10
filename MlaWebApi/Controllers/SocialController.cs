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
                    username = Convert.ToString(row["username"]).Trim(),
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

            SqlCommand comm = new SqlCommand($"SELECT T3.user_id, T3.publicKey, T3.fullname, T1.group_id, group_name, group_key, T1.gk_signature FROM sn_group_key_table T1 INNER JOIN sn_group T2 ON T1.group_id = T2.group_id INNER JOIN sn_user T3 ON T1.key_author_id = T3.user_id where T1.member_id = {memberId}", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetGroupsByMemberIdModel
                {
                    group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    owner_fullname = Convert.ToString(row["fullname"]).Trim(),
                    group_owner_id = Convert.ToString(row["user_id"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    signature = Convert.ToString(row["gk_signature"]).Trim(),
                    grp_ownrs_pub_key = Convert.ToString(row["publicKey"]).Trim()
                };
            }
        }

        public IEnumerable<GetGroupsByMemberIdModel> GetGroupsByNotAMemberId(int notJoinedMemberId)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand($"SELECT T3.user_id, T3.publicKey, T3.fullname, T1.group_id, group_name, group_key, T1.gk_signature FROM sn_group_key_table T1 INNER JOIN sn_group T2 ON T1.group_id = T2.group_id INNER JOIN sn_user T3 ON T1.key_author_id = T3.user_id where T1.member_id != '{notJoinedMemberId}'", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups2");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetGroupsByMemberIdModel
                {
                    group_owner_id = Convert.ToString(row["user_id"]).Trim(),
                    group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    signature = Convert.ToString(row["gk_signature"]).Trim(),
                    public_key = Convert.ToString(row["publicKey"]).Trim(),
                    owner_fullname = Convert.ToString(row["fullname"]).Trim()
                };
            }
        }

        // OLD GET POSTS QUERY
        //$"select T1.author_id, T4.fullname, T1.group_id, group_name, group_key, " +
        //$"post_data, post_key, timestamp " +
        //$"from sn_posts T1 " +

        //$"inner join sn_group_key_table T2 on T2.group_id = T1.group_id " +
        //$"inner join sn_group T3 on T2.group_id = T3.group_id " +
        //$"inner join sn_user T4 on T4.user_id = '{userId2}' " +

        //$"where T1.group_id in (" +
        //    $" select T1.group_id" +
        //    $" from sn_group_key_table T1" +
        //        $" inner join sn_group T2 on T1.group_id = T2.group_id" +
        //        $" where T1.member_id = {userId2}) " +
        //    $"and T2.member_id = {userId2}"

        // Get groups the user is part of
        // Get posts made in these groups
        public IEnumerable<GetPostsByMemberIdModel> GetPosts(string userId2)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand($"select * from sn_posts T1 inner join sn_user T2 on T1.author_id = T2.user_id inner join sn_group on T1.group_id = sn_group.group_id inner join sn_group_key_table T3 on T1.group_id = T3.group_id and T3.member_id = '{userId2}' where T1.group_id in (select group_id from sn_group_key_table where member_id = '{userId2}')", cnn);

            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("posts");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetPostsByMemberIdModel
                {
                    author_id = Convert.ToString(row["author_id"]).Trim(),
                    group_id = Convert.ToString(row["group_id"]).Trim(),
                    post_data = Convert.ToString(row["post_data"]).Trim(),
                    timestamp = Convert.ToString(row["timestamp"]).Trim(),
                    post_key = Convert.ToString(row["post_key"]).Trim(),
                    fullname = Convert.ToString(row["fullname"]).Trim(),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    signature = Convert.ToString(row["gk_signature"]).Trim(),
                    ownr_public_key = Convert.ToString(row["publicKey"]).Trim()
                };
            }
        }

        // auto fills other required values as this is one time per user
        public HttpResponseMessage RegisterNewUser(
            string username,
            string password,
            string publicKeyString,
            string fullname,
            string encryptedGroupKey,
            string signature)
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                String commandText = $"insert into sn_user (username, password, publicKey, fullname) values ('{username}', '{password}', '{publicKeyString}', '{fullname}')";

                SqlCommand comm = new SqlCommand(commandText, cnn);
                SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
                DataSet dsDatast = new DataSet("newUser");
                Sqlda.Fill(dsDatast);

                SqlCommand comm2 = new SqlCommand($"Select user_id from sn_user where username = '{username}'", cnn);
                SqlDataAdapter Sqlda2 = new SqlDataAdapter(comm2);
                dsData = new DataSet();
                Sqlda2.Fill(dsData);

                int userId = Int32.Parse(dsData.Tables[0].Rows[0]["user_id"].ToString());

                SqlCommand comm3 = new SqlCommand($"insert into sn_group (group_name, group_owner_id) values ('friends_group', '{userId}')", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                SqlCommand comm4 = new SqlCommand($"Select group_id from sn_group where group_name = 'friends_group' and group_owner_id = '{userId}'", cnn);
                SqlDataAdapter Sqlda4 = new SqlDataAdapter(comm4);
                dsData = new DataSet();
                Sqlda4.Fill(dsData);

                int groupId = Int32.Parse(dsData.Tables[0].Rows[0]["group_id"].ToString());

                SqlCommand comm5 = new SqlCommand($"insert into sn_group_key_table (owner_id, member_id, group_key, group_id, key_version, key_status, gk_signature, key_author_id) values ('{userId}', '{userId}', '{encryptedGroupKey}', '{groupId}', '1', '0', '{signature}', '{userId}')", cnn);

                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "User added id: " + userId + " friends group id : " + groupId);

                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Violation of PRIMARY KEY"))
                {
                    var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "User Already Exists ");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, "Bad Request" + e.ToString());
                    cnn.Close();
                    return response;
                }
            }
        }

        public HttpResponseMessage CreateNewGroup(
           string owner_id,
           string group_name,
           string encryptedGroupKey,
           string signature
            )
        {

            DataSet dsData = new DataSet("user");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {

                SqlCommand comm3 = new SqlCommand($"insert into sn_group (group_name, group_owner_id) values ('{group_name}', '{owner_id}')", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                SqlCommand comm4 = new SqlCommand($"Select group_id from sn_group where group_name = '{group_name}' and group_owner_id = '{owner_id}'", cnn);
                SqlDataAdapter Sqlda4 = new SqlDataAdapter(comm4);
                dsData = new DataSet();
                Sqlda4.Fill(dsData);

                int groupId = Int32.Parse(dsData.Tables[0].Rows[0]["group_id"].ToString());

                SqlCommand comm5 = new SqlCommand($"insert into sn_group_key_table (owner_id, member_id, group_key, group_id, key_version, key_status, gk_signature, key_author_id) values ('{owner_id}', '{owner_id}', '{encryptedGroupKey}', '{groupId}', '1', '0', '{signature}', '{owner_id}')", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "Group created id: " + groupId);
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
          string group_owner_id,
          string join_request_hash,
          string signature
           )
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm3 = new SqlCommand($"insert into sn_add_requests (user_id, group_id, group_owner_id, join_request_hash, signature) values ('{user_id}', '{group_id}', '{group_owner_id}', '{join_request_hash}', '{signature}')", cnn);

                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                DataSet dsData = new DataSet();
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

            SqlCommand comm3 = new SqlCommand($"SELECT T1.user_id, T1.join_request_hash, T1.signature, T3.fullname, T3.publicKey as requesters_pub_key, T5.publicKey as group_owners_pub_key, T1.group_id, T2.group_key, T4.group_name, T1.group_owner_id, T2.gk_signature FROM sn_add_requests T1 INNER JOIN sn_user T3 ON T1.user_id = T3.user_id INNER JOIN sn_user T5 ON T5.user_id = T1.group_owner_id INNER JOIN sn_group_key_table T2 ON T1.group_id = T2.group_id INNER JOIN sn_group T4 ON T1.group_id = T4.group_id where T1.group_owner_id = '{user_id}'", cnn);

            SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
            DataSet psDataSet = new DataSet("pendingAddRequests");
            Sqlda3.Fill(psDataSet);

            foreach (DataRow row in psDataSet.Tables[0].Rows)
            {
                yield return new GetPendingAddRequestsModel
                {

                    requester_id = Convert.ToString(row["user_id"]).Trim(),
                    requester_fullname = Convert.ToString(row["fullname"]).Trim(),
                    requesters_pub_key = Convert.ToString(row["requesters_pub_key"]).Trim(),
                    group_id = Convert.ToString(row["group_id"]).Trim(),
                    group_key = Convert.ToString(row["group_key"]).Trim(),
                    group_name = Convert.ToString(row["group_name"]).Trim(),
                    group_owner_id = Convert.ToString(row["group_owner_id"]).Trim(),
                    join_request_hash = Convert.ToString(row["join_request_hash"]).Trim(),
                    signature = Convert.ToString(row["signature"]).Trim()

                };
            }
        }

        public HttpResponseMessage ApproveGroupRequest(
           string group_owner_id,
           string encryptedGroupKey,
           string requester_id_for_approve,
           string group_id,
           string signature)
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm5 = new SqlCommand($"insert into sn_join_confirmation_table (group_owner_id, requester_id, encryptedGroupKey, group_id, signature) values ('{group_owner_id}', '{requester_id_for_approve}', '{encryptedGroupKey}', '{group_id}', '{signature}')", cnn);

                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                DataSet dsData = new DataSet();
                Sqlda5.Fill(dsData);

                SqlCommand comm6 = new SqlCommand($"delete from sn_add_requests where group_owner_id = '{group_owner_id}' and user_id = '{requester_id_for_approve}' and group_id = '{group_id}'", cnn);

                SqlDataAdapter Sqlda6 = new SqlDataAdapter(comm6);
                DataSet dsData2 = new DataSet();
                Sqlda6.Fill(dsData2);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, $"User {requester_id_for_approve} will be added to Group {group_id}!");
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

        public HttpResponseMessage DenyAddRequest(
          string requester_id,
          string group_id,
          string group_owner_id
           )
        {
            DataSet dsData = new DataSet("denyAddRequest");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm3 = new SqlCommand($"delete from sn_add_requests where user_id = {requester_id} and group_id = {group_id} and group_owner_id = {group_owner_id}", cnn);
                SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
                dsData = new DataSet();
                Sqlda3.Fill(dsData);

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "Add Request Discarded !");

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

        public IEnumerable<GetJoinConfirmationsModel> GetJoinConfirmations(int user_idForJoinConfirmation)
        {
            
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm3 = new SqlCommand($"select T1.group_owner_id, T1.requester_id, T1.group_id, T1.encryptedGroupKey, T1.signature, T2.publicKey from sn_join_confirmation_table T1 inner join sn_user T2 on T1.group_owner_id = T2.user_id where T1.requester_id = { user_idForJoinConfirmation}", cnn);
            SqlDataAdapter Sqlda3 = new SqlDataAdapter(comm3);
            DataSet dsData3 = new DataSet();
            Sqlda3.Fill(dsData3);

            foreach (DataRow row in dsData3.Tables[0].Rows)
            {
                yield return new GetJoinConfirmationsModel
                {

                    requester_id = Convert.ToString(row["requester_id"]).Trim(),
                    encryptedGroupKey = Convert.ToString(row["encryptedGroupKey"]).Trim(),
                    group_id = Convert.ToString(row["group_id"]).Trim(),
                    signature = Convert.ToString(row["signature"]).Trim(),
                    group_owner_id = Convert.ToString(row["group_owner_id"]).Trim(),
                    public_key = Convert.ToString(row["publicKey"]).Trim()

                };
            }

            cnn.Close();
        }

        public HttpResponseMessage FinalizeJoin(
          string requester_id,
          string group_id,
          string group_owner_id,
          string encryptedGroupKey,
          string signature
           )
        {
            DataSet dsData = new DataSet("finalizeJoin");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                // key_version needs to be changed in leave group functionality
                SqlCommand comm5 = new SqlCommand($"insert into sn_group_key_table (owner_id, member_id, group_key, group_id, key_version, key_status, gk_signature, key_author_id) values ('{group_owner_id}', '{requester_id}', '{encryptedGroupKey}', '{group_id}', '1', '0', '{signature}', '{group_owner_id}')", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                SqlCommand comm7 = new SqlCommand($"delete from sn_join_confirmation_table where group_owner_id = '{group_owner_id}' and requester_id = '{requester_id}' and encryptedGroupKey = '{encryptedGroupKey}' and group_id = '{group_id}'", cnn);
                SqlDataAdapter Sqlda7 = new SqlDataAdapter(comm7);
                DataSet dsData7 = new DataSet();
                Sqlda7.Fill(dsData7);

                SqlCommand comm4 = new SqlCommand($"Select group_name from sn_group_key_table T1 inner join sn_group T2 on T1.group_id = T2.group_id where T1.group_id = '{group_id}' and owner_id = '{group_owner_id}'", cnn);
                SqlDataAdapter Sqlda4 = new SqlDataAdapter(comm4);
                dsData = new DataSet();
                Sqlda4.Fill(dsData);

                String group_name = dsData.Tables[0].Rows[0]["group_name"].ToString();

                var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "Your request to join the group " + group_name + " (ID: " + group_id + ") has been accepted!");

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

    }
}
