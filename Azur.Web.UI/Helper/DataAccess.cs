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

            SqlCommand oSqlCommand = new SqlCommand("uspUserGet", oSqlConnection);
            oSqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
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

        /// <summary>
        /// Add User
        /// </summary>
        /// <param name="p_oUserModel"></param>
        /// <returns></returns>
        public bool Add(UserModel p_oUserModel)
        {
            try
            {
                Connection();
                SqlCommand oSqlCommand = new SqlCommand("uspUserAdd", oSqlConnection);
                oSqlCommand.CommandType = CommandType.StoredProcedure;

                oSqlCommand.Parameters.AddWithValue("@p_strFullName", p_oUserModel.FullName);
                oSqlCommand.Parameters.AddWithValue("@p_strTitle", p_oUserModel.Title);
                oSqlCommand.Parameters.AddWithValue("@p_strUserName", p_oUserModel.Username);
                oSqlCommand.Parameters.AddWithValue("@p_strPassword", p_oUserModel.Password);
                oSqlCommand.Parameters.AddWithValue("@p_strEmail", p_oUserModel.Email);
                oSqlCommand.Parameters.AddWithValue("@p_bIsActive", p_oUserModel.IsActive);
                oSqlCommand.Parameters.AddWithValue("@p_bIsDeleted", 0);
                oSqlCommand.Parameters.AddWithValue("@p_iVersion", 1);
                oSqlCommand.Parameters.AddWithValue("@p_dtCreateDate", DateTime.Now);
                oSqlCommand.Parameters.AddWithValue("@p_iCreateByUserId", 0);
                oSqlCommand.Parameters.AddWithValue("@p_dtUpdateDate", DateTime.Now);
                oSqlCommand.Parameters.AddWithValue("@p_iUpdateByUserId", 0);

                oSqlConnection.Open();
                int i = oSqlCommand.ExecuteNonQuery();
                oSqlConnection.Close();

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception oException)
            {
                throw oException;
            }
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="p_oUserModel"></param>
        /// <returns></returns>
        public bool Update(UserModel p_oUserModel)
        {
            try
            {
                Connection();
                SqlCommand oSqlCommand = new SqlCommand("uspUserUpdate", oSqlConnection);
                oSqlCommand.CommandType = CommandType.StoredProcedure;

                oSqlCommand.Parameters.AddWithValue("@p_iUserId", p_oUserModel.UserId);
                oSqlCommand.Parameters.AddWithValue("@p_strFullName", p_oUserModel.FullName);
                oSqlCommand.Parameters.AddWithValue("@p_strTitle", p_oUserModel.Title);
                oSqlCommand.Parameters.AddWithValue("@p_strUserName", p_oUserModel.Username);
                oSqlCommand.Parameters.AddWithValue("@p_strPassword", p_oUserModel.Password);
                oSqlCommand.Parameters.AddWithValue("@p_strEmail", p_oUserModel.Email);
                oSqlCommand.Parameters.AddWithValue("@p_bIsActive", p_oUserModel.IsActive);
                oSqlCommand.Parameters.AddWithValue("@p_iVersion", p_oUserModel.Version);
                oSqlCommand.Parameters.AddWithValue("@p_dtCreateDate", DateTime.Now);
                oSqlCommand.Parameters.AddWithValue("@p_iCreateByUserId", 0);
                oSqlCommand.Parameters.AddWithValue("@p_dtUpdateDate", DateTime.Now);
                oSqlCommand.Parameters.AddWithValue("@p_iUpdateByUserId", 0);

                oSqlConnection.Open();
                int i = oSqlCommand.ExecuteNonQuery();
                oSqlConnection.Close();

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception oException)
            {
                throw oException;
            }
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="p_oUserModel"></param>
        /// <returns></returns>
        public bool Delete(UserModel p_oUserModel)
        {
            try
            {
                Connection();
                SqlCommand oSqlCommand = new SqlCommand("uspUserDelete", oSqlConnection);
                oSqlCommand.CommandType = CommandType.StoredProcedure;

                oSqlCommand.Parameters.AddWithValue("@p_iUserId", p_oUserModel.UserId);
                oSqlCommand.Parameters.AddWithValue("@p_bIsDeleted", 1);
                oSqlCommand.Parameters.AddWithValue("@p_dtUpdateDate", DateTime.Now);
                oSqlCommand.Parameters.AddWithValue("@p_iUpdateByUserId", 0);

                oSqlConnection.Open();
                int i = oSqlCommand.ExecuteNonQuery();
                oSqlConnection.Close();

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception oException)
            {
                throw oException;
            }
        }

        /// <summary>
        /// Get Data User
        /// </summary>
        /// <returns></returns>
        public List<StorageModel> GetStorage()
        {
            Connection();
            List<StorageModel> lsData = new List<StorageModel>();

            SqlCommand oSqlCommand = new SqlCommand("uspStorageGet", oSqlConnection);
            oSqlCommand.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter oSqlDataAdapter = new SqlDataAdapter(oSqlCommand);
            DataTable dt = new DataTable();

            oSqlConnection.Open();
            oSqlDataAdapter.Fill(dt);
            oSqlConnection.Close();

            foreach (DataRow oDataRow in dt.Rows)
            {
                lsData.Add(
                    new StorageModel
                    {
                        StorageId = Convert.ToInt32(oDataRow["KTI_STORAGE_ID"]),
                        ImageName = Convert.ToString(oDataRow["IMAGE_NAME"]),
                        Caption = Convert.ToString(oDataRow["CAPTION"]),
                        ImageUri = Convert.ToString(oDataRow["IMAGE_URI"]),
                        ThumbnailUri = Convert.ToString(oDataRow["THUMBNAIL_URI"]),
                        Response = Convert.ToString(oDataRow["RESPONSE"])
                    });
            }

            return lsData;
        }

        /// <summary>
        /// Add Storage
        /// </summary>
        /// <param name="p_oStorageModel"></param>
        /// <returns></returns>
        public bool AddStorage(StorageModel p_oStorageModel)
        {
            try
            {
                Connection();
                SqlCommand oSqlCommand = new SqlCommand("uspStorageAdd", oSqlConnection);
                oSqlCommand.CommandType = CommandType.StoredProcedure;

                oSqlCommand.Parameters.AddWithValue("@p_strImageName", p_oStorageModel.ImageName);
                oSqlCommand.Parameters.AddWithValue("@p_strCaption", p_oStorageModel.Caption);
                oSqlCommand.Parameters.AddWithValue("@p_strImageUri", p_oStorageModel.ImageUri);
                oSqlCommand.Parameters.AddWithValue("@p_strThumbnailUri", p_oStorageModel.ThumbnailUri);
                oSqlCommand.Parameters.AddWithValue("@p_strResponse", p_oStorageModel.Response);

                oSqlConnection.Open();
                int i = oSqlCommand.ExecuteNonQuery();
                oSqlConnection.Close();

                if (i >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception oException)
            {
                throw oException;
            }
        }
    }
}