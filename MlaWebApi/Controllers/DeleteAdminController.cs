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
    public class DeleteAdminController : ApiController
    {

        public string cfmgr = ConfigurationManager.ConnectionStrings["MlaDatabase"].ConnectionString;
        SqlConnection cnn = null;

        public HttpResponseMessage PostAdminRmv(string idAdminRmv)
        {
            DataSet dsData = new DataSet("tasks");
            cnn = new SqlConnection(cfmgr);
            cnn.Open();
            try
            {
                SqlCommand comm9 = new SqlCommand("DELETE FROM admin where idAdmin ='" + idAdminRmv + "'", cnn);
                SqlDataAdapter Sqlda9 = new SqlDataAdapter(comm9);
                dsData = new DataSet();
                Sqlda9.Fill(dsData);

                SqlCommand comm10 = new SqlCommand("DELETE FROM register where userName ='" + idAdminRmv + "'", cnn);
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
