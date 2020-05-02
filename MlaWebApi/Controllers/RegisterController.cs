using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MlaWebApi.Models;
////using System.Data.SqlServerCe;
using System.Data;
using System.Configuration;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace MlaWebApi.Controllers
{
    public class RegisterController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public IEnumerable<Register> GetAllRegister()
        {
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select userId, userName, userType from register", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);
            DataSet dsDatast = new DataSet("register");
            Sqlda.Fill(dsDatast);

            foreach (DataRow row in dsDatast.Tables[0].Rows)
            {
                yield return new Register
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"])),
                    userName = Convert.ToString(row["userName"]),
                    userType = Convert.ToString(row["userType"])
                };
            }

        }

        public IEnumerable<Register> GetRegisterByUserName(string userName)
        {
           
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select userId,userName,userType from register where userName = '" + userName + "'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("register");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Register
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"])),
                    userName = Convert.ToString(row["userName"]),
                    userType = Convert.ToString(row["userType"])
                };
            }
        }
      
        public IEnumerable<Register> GetRegisterAuth(string userName,string password)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select userId,userName,userType from register where userName = '" + userName + "'"+ " and password = '"+password+"'", cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("register");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Register
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"])),
                    userName = Convert.ToString(row["userName"]),
                    userType = Convert.ToString(row["userType"])
                };
            }
        }

        public IEnumerable<Register> GetRegisterByUserId(int userId)
        {

            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            SqlCommand comm = new SqlCommand("Select userId,userName,userType from register where userId = " + userId, cnn);
            SqlDataAdapter Sqlda = new SqlDataAdapter(comm);

            DataSet dataSet = new DataSet("register");
            Sqlda.Fill(dataSet);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                yield return new Register
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"])),
                    userName = Convert.ToString(row["userName"]),
                    userType = Convert.ToString(row["userType"])
                };
            }
        }

        public HttpResponseMessage PostRegisterPassUpdate(string userName, string password)
        {

            DataSet dsData = new DataSet("register");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            Register register = new Register();
            register.userName = userName;
            try
            {
                SqlCommand comm = new SqlCommand("Update register set password ='" + password +"'"+" where userName = '"+userName+"'", cnn);
                //int countUpdated =comm.ExecuteNonQuery();
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);
            //    comm.ExecuteNonQuery();
              //  comm.Dispose();

                var response = Request.CreateResponse<Register>(System.Net.HttpStatusCode.Found, register);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Register>(System.Net.HttpStatusCode.BadRequest, register);
                cnn.Close();
                return response;
            }
            
        }

        public HttpResponseMessage PostRegister(Register register)
        {

            DataSet dsData = new DataSet("register");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();

            try
            {
                SqlCommand comm = new SqlCommand("Insert into register(userName,password,userType) values('"
                    + register.userName
                    + "','" + register.password
                    + "','" + register.userType
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                var response = Request.CreateResponse<Register>(System.Net.HttpStatusCode.Created, register);

                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Register>(System.Net.HttpStatusCode.BadRequest, register);
                return response;
            }

        }

        public HttpResponseMessage PostAddInstructor(string instUserName, string instPassword, string instFirsName, string instLastName, string instTelephone, string instAddress, string instAliasMailId, string instEmailId, string instSkypeId)
        {
            DataSet dsData = new DataSet("register");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            string userType = "instructor"; // userType = instructor or student or admin
            int userId = 0;

            //first add to register table then to the instructor table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into register(userName,password,userType) values('"
                    + instUserName
                    + "','" + instPassword
                    + "','" + userType
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                // retrive the userId since it is auto incremented in the database and need to be added to the instructor table
                comm = new SqlCommand("select userId from register where userName = '" + instUserName + "'",cnn);
                sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);


                foreach (DataRow row in dsData.Tables[0].Rows)
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"]));
                }

            }
            catch (Exception e)
            {
                Instructor emptyInst = new Instructor();
                var response = Request.CreateResponse<Instructor>(System.Net.HttpStatusCode.BadRequest, emptyInst);
                return response;
            }

            Instructor inst = new Instructor();
            inst.idInstructor = instUserName;
            inst.firstName = instFirsName;
            inst.lastName = instLastName;
            inst.userId = userId;
            inst.telephone = instTelephone;
            inst.address = instAddress;
            inst.aliasMailId = instAliasMailId;
            inst.emailId = instEmailId;
            inst.skypeId = instSkypeId;
            // now add to instructor table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into instructor(idInstructor,firstName,lastName,userId,telephone,address,aliasMailId,emailId, skypeId) values('"
                    + inst.idInstructor
                    + "','" + inst.firstName
                    + "','" + inst.lastName
                    + "','" + inst.userId
                    + "','" + inst.telephone
                    + "','" + inst.address
                    + "','" + inst.aliasMailId
                    + "','" + inst.emailId
                    + "','" + inst.skypeId
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);
                cnn.Close();
                var response = Request.CreateResponse<Instructor>(System.Net.HttpStatusCode.Created, inst);
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Instructor>(System.Net.HttpStatusCode.BadRequest, inst);
                cnn.Close();
                return response;
            }
        }

        public HttpResponseMessage PostAddStudent(string userName, string password, string firsName, string lastName, string telephone, string address, string aliasMailId, string emailId, string skypeId)
        {
            DataSet dsData = new DataSet("register");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            string userType = "student"; // userType = instructor or student or admin
            int userId = 0;

            // first add to register table then to the student table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into register(userName,password,userType) values('"
                    + userName
                    + "','" + password
                    + "','" + userType
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                // retrive the userId since it is auto incremented in the database and need to be added to the student table
                comm = new SqlCommand("select userId from register where userName = '" + userName + "'",cnn);
                sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);


                foreach (DataRow row in dsData.Tables[0].Rows)
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"]));
                }

            }
            catch (Exception e)
            {
                Student emptyStud = new Student();
                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.BadRequest, emptyStud);
                cnn.Close();
                return response;
            }

            Student stud = new Student();
            stud.idStudent = userName;
            stud.firstName = firsName;
            stud.lastName = lastName;
            stud.userId = userId;
            stud.telephone = telephone;
            stud.address = address;
            stud.aliasMailId = aliasMailId;
            stud.emailId = emailId;
            stud.skypeId = skypeId;
            // now add to instructor table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into student(idStudent,firstName,lastName,userId,telephone,address,aliasMailId,emailId,skypeId) values('"
                    + stud.idStudent
                    + "','" + stud.firstName
                    + "','" + stud.lastName
                    + "','" + stud.userId
                    + "','" + stud.telephone
                    + "','" + stud.address
                    + "','" + stud.aliasMailId
                    + "','" + stud.emailId
                    + "','" + stud.skypeId
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);
                cnn.Close();
                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.Created, stud);
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.BadRequest, stud);
                cnn.Close();
                return response;
            }
        }

        public HttpResponseMessage PostAddAdmin(string adminUserName, string adminPassword, string adminFirsName, string adminLastName, string adminTelephone, string adminAddress, string adminAliasMailId, string adminEmailId, string adminSkypeId)
        {
            DataSet dsData = new DataSet("admin");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            string userType = "admin"; // userType = instructor or student or admin
            int userId = 0;
         //  MessageBox.Show(adminUserName);
           //Console.WriteLine(adminUserName);
            //first add to register table then to the student table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into register(userName,password,userType) values('"
                   + adminUserName
                   + "','" + adminPassword
                   + "','" + userType
                   + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                // retrive the userId since it is auto incremented in the database and need to be added to the student table
                comm = new SqlCommand("select userId from register where userName = '" + adminUserName + "'",cnn);
                sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);

                foreach (DataRow row in dsData.Tables[0].Rows)
                {
                    userId = Int16.Parse(Convert.ToString(row["userId"]));
                }
              //  MessageBox.Show(userId+"");
            }
            catch (SqlException e)
            {
               // MessageBox.Show(e.Message);
                Student emptyAdmin = new Student();
                var response = Request.CreateResponse<Student>(System.Net.HttpStatusCode.BadRequest, emptyAdmin);
                cnn.Close();
                return response;
            }

            Admin adm = new Admin();
            adm.idAdmin = adminUserName;
            adm.firstName = adminFirsName;
            adm.lastName = adminLastName;
            adm.userId = userId;
            adm.telephone = adminTelephone;
            adm.address = adminAddress;
            adm.aliasMailId = adminAliasMailId;
            adm.emailId = adminEmailId;
            adm.skypeId = adminSkypeId;
            // now add to instructor table.
            try
            {
                SqlCommand comm = new SqlCommand("Insert into admin(idAdmin,firstName,lastName,userId,telephone,address,aliasMailId,emailId,skypeId) values('"
                    + adm.idAdmin
                    + "','" + adm.firstName
                    + "','" + adm.lastName
                    + "','" + adm.userId
                    + "','" + adm.telephone
                    + "','" + adm.address
                    + "','" + adm.aliasMailId
                    + "','" + adm.emailId
                    + "','" + adm.skypeId
                    + "')", cnn);
                SqlDataAdapter sqlada = new SqlDataAdapter(comm);
                sqlada.Fill(dsData);
                cnn.Close();
                var response = Request.CreateResponse<Admin>(System.Net.HttpStatusCode.Accepted, adm);
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Admin>(System.Net.HttpStatusCode.BadRequest, adm);
                cnn.Close();
                return response;
            }
        }
        
    }
}
