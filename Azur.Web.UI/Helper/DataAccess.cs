using Azur.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

#region Trademark

/**
 *  This software, all associated documentation, and all copies are CONFIDENTIAL INFORMATION of Kalpawreska Teknologi Indonesia
 *  http://www.fwahyudianto.id
 *  ® Wahyudianto, Fajar
 *  Email 	: fwahyudi06@gmail.com
 */

#endregion // end of Trademark

namespace Azur.Web.UI.Helper
{
    public class DataAccess
    {
        private SqlConnection oSqlConnection;

        /// <summary>
        /// Default Connection
        /// </summary>
        private void Connection()
        {
            string strAppId = ConfigurationManager.AppSettings.Get("AppId");
            string strConnectionString = ConfigurationManager.AppSettings.Get("ConnectionString").Trim();
            string strConnectionTimeout = ConfigurationManager.AppSettings.Get("ConnectionTimeout");

            oSqlConnection = new SqlConnection(strConnectionString);
        }

        /// <summary>
        /// Get Data User
        /// </summary>
        /// <returns></returns>
        public List<UserModel> Get()
        {
            Connection();
            List<UserModel> lsData = new List<UserModel>();

            SqlCommand cmd = new SqlCommand("uspUserGet", oSqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter oSqlDataAdapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            oSqlConnection.Open();
            oSqlDataAdapter.Fill(dt);
            oSqlConnection.Close();

            foreach (DataRow oDataRow in dt.Rows)
            {
                lsData.Add(
                    new UserModel
                    {
                        UserId = Convert.ToInt32(oDataRow["KTI_USER_ID"]),
                        FullName = Convert.ToString(oDataRow["FULL_NAME"]),
                        Title = Convert.ToString(oDataRow["TITLE"]),
                        Username = Convert.ToString(oDataRow["USER_NAME"]),
                        Password = Convert.ToString(oDataRow["PASSWORD"]),
                        Email = Convert.ToString(oDataRow["EMAIL"]),
                        IsActive = Convert.ToBoolean(oDataRow["IS_ACTIVE"]),
                        IsDeleted = Convert.ToBoolean(oDataRow["IS_DELETED"]),
                        Version = Convert.ToInt32(oDataRow["VERSION"]),
                        CreateDate = Convert.ToDateTime(oDataRow["CREATE_DATE"]),
                        CreateBy = Convert.ToInt32(oDataRow["CREATE_BY_USER_ID"]),
                        UpdateDate = Convert.ToDateTime(oDataRow["UPDATE_DATE"]),
                        UpdateBy = Convert.ToInt32(oDataRow["UPDATE_BY_USER_ID"])
                    });
            }

            return lsData;
        }
    }
}