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
    public class ScheduleRmvController : ApiController
    {
        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public HttpResponseMessage PostTaskRmv(string subject_id)
        {
            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            try
            {
                SqlCommand comm5 = new SqlCommand("DELETE FROM student_tasks where subject_id ='" + subject_id + "'", cnn);
                SqlDataAdapter Sqlda5 = new SqlDataAdapter(comm5);
                dsData = new DataSet();
                Sqlda5.Fill(dsData);

                SqlCommand comm6 = new SqlCommand("DELETE FROM tasks where subject_id ='" + subject_id + "'", cnn);
                SqlDataAdapter Sqlda6 = new SqlDataAdapter(comm6);
                dsData = new DataSet();
                Sqlda6.Fill(dsData);

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
