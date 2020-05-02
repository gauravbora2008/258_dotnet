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
    public class DeleteStudentController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public HttpResponseMessage PostStudentRmv(string idStudentRmv)
        {
            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            try
            {
                SqlCommand comm5 = new SqlCommand("DELETE FROM student_tasks where student_id ='" + idStudentRmv + "'", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                SqlCommand comm8 = new SqlCommand("DELETE FROM subject_roster where student_id='" + idStudentRmv + "'", cnn);
                SqlDataAdapter Sqlda8 = new SqlDataAdapter(comm8);
                dsData = new DataSet();
                Sqlda8.Fill(dsData);



                SqlCommand comm9 = new SqlCommand("DELETE FROM student where idStudent ='" + idStudentRmv + "'", cnn);
                SqlDataAdapter Sqlda9 = new SqlDataAdapter(comm9);
                dsData = new DataSet();
                Sqlda9.Fill(dsData);

                SqlCommand comm10 = new SqlCommand("DELETE FROM register where userName ='" + idStudentRmv + "'", cnn);
                SqlDataAdapter Sqlda10 = new SqlDataAdapter(comm10);
                dsData = new DataSet();
                Sqlda10.Fill(dsData);

                var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.Found, null);
                cnn.Close();
                return response;
            }
            catch (Exception e)
            {
                var response = Request.CreateResponse<Tasks>(System.Net.HttpStatusCode.BadRequest, null);
                cnn.Close();
                return response;
            }
        }
    }
}
