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

            SqlCommand comm = new SqlCommand($"Select user_id, fullname, publicKey from sn_user where username = '{username}' and password = '{password}'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet loginDataSet = new DataSet("userLogin");
            Sqlda.Fill(loginDataSet);

            // Check if the result of the query is null
            // Else :
            /* - find out all the groups that I am part of (select group_id from sn_group where member_id = user_id )
             * 
             *      1) for each group_id: check group statuses for latest version keys if there are any invalid keys (
             *          SELECT MAX(key_version) AS Latest_Key_Version, group_id FROM [MlaDatabase].[dbo].[sn_group_key_table] where             key_status = 1 and group_id in (1,2, 3) GROUP BY group_id 
             *          
             *          if any group has key_status = 1 i.e. invalid, add a row for each member with new keys
             *          
             *          Response: the group_ids, current_key_version public_keys of group members:
             *              owner_id = 1
             *              current_key_version = 3
             *              group_id = 1
             *              member_id = 43
             *              public_key = '43spublickey' (select publickey from sn_user where user_id = member_id) consider join?
             *              
             *           App action: for each such record, generate a new group key, use member's public key to encrypt the group                         key and create a record to be sent back:
             *              owner_id = 1
             *              member_id = 43
             *              enc_group_key = 'ad98983h08fh093'
             *              group_id = 1
             *              key_version = current_key_version + 1
             *              key_status = 0 (clean)
             *              
             *              (there should be only one 0 for a members all keys in a group_id)
             *              
             *           server Action: insert into group_key_table values (owner_id, member_id, enc_group_key, group_id,       
             *                                           key_version, key_status)
             *                                           
             *                                           
             *                                           
             *      2) get posts - select author_id, fullname, group_id, post_data, post_key from poststable where member_id = userid
             *      
             *      3) write posts - 1) get groupkey for the group i am posting in 
             *                       2) generate a post_key
             *                       3) encrypt the post with post_key
             *                       4) encrypt the post_key with group_key
             *                       
             *               POST : author_id = user_id
             *                      group_id = group_id in which post is being made
             *                      post_data = post text encrypted with post_key
             *                      post_key = post_key string encrypted with group_key
             *                      timestamp = time of post
             *                      
             *      4) create group
             *      
             *      5) join group
             *      
             *      6) add friend
             *      
             *      
             *       post visibility - later
            
            */

            foreach (DataRow row in loginDataSet.Tables[0].Rows)
            {
                yield return new SN_UserLogin
                {
                    user_id = Int16.Parse(Convert.ToString(row["user_id"])),
                    fullname = Convert.ToString(row["fullname"]),
                    publicKey = Convert.ToString(row["publicKey"])
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

            SqlCommand comm = new SqlCommand($"Select group_id, group_name from sn_group where member_id = {memberId}", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("groups");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new GetGroupsByMemberIdModel
                {
                    group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
                    group_name = Convert.ToString(row["group_name"]).Trim()
                };
            }
        }

        //public IEnumerable<GetGroupsByMemberIdModel> CreateGroup(int ownerId)
        //{
        //    cnn = new SqlConnection(cfmgr);
        //    cnn.Open();

        //    SqlCommand comm = new SqlCommand($"insert into group_id, group_name from sn_group where member_id = {ownerId}", cnn);
        //    SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
        //    DataSet dsDatast = new DataSet("groups");
        //    Sqlda.Fill(dsDatast);

        //    foreach (DataRow row in dsDatast.Tables[0].Rows)
        //    {
        //        yield return new GetGroupsByMemberIdModel
        //        {
        //            group_id = Int32.Parse(Convert.ToString(row["group_id"]).Trim()),
        //            group_name = Convert.ToString(row["group_name"]).Trim()
        //        };
        //    }
        //}


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
                    var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, "User Already Exists");
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
