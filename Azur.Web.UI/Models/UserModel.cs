using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data;

#region Trademark

/**
 *  This software, all associated documentation, and all copies are CONFIDENTIAL INFORMATION of Kalpawreska Teknologi Indonesia
 *  http://www.fwahyudianto.id
 *  ® Wahyudianto, Fajar
 *  Email 	: fwahyudi06@gmail.com
 */

#endregion // end of Trademark

namespace Azur.Web.UI.Models
{
    public class UserModel
    {
        #region Variable

        protected int m_iUserId = -99;
        protected string m_strFullName = string.Empty;
        protected string m_strTitle = string.Empty;
        protected string m_strUsername = string.Empty;
        protected string m_strPassword = string.Empty;
        protected string m_strEmail = string.Empty;
        protected bool m_bIsActive = false;
        protected bool m_bIsDeleted = false;
        protected int m_iVersion = -99;
        protected int m_iCreateByUserId = -99;
        protected DateTime m_dtCreateDate = new DateTime(1900, 1, 1);
        protected int m_iUpdateByUserId = -99;
        protected DateTime m_dtUpdateDate = new DateTime(1900, 1, 1);

        #endregion // end of Variable

        #region Property

        [Display(Name = "Id")]
        public int UserId
        {
            get { return m_iUserId; }
            set { m_iUserId = value; }
        }

        [Required(ErrorMessage = "Full Name is Required!")]
        public string FullName
        {
            get { return m_strFullName; }
            set { m_strFullName = value; }
        }

        [Required(ErrorMessage = "Title is Required!")]
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        [Required(ErrorMessage = "Username is Required!")]
        public string Username
        {
            get { return m_strUsername; }
            set { m_strUsername = value; }
        }

        [Required(ErrorMessage = "Password is Required!")]
        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        [Required(ErrorMessage = "Email is Required!")]
        public string Email
        {
            get { return m_strEmail; }
            set { m_strEmail = value; }
        }

        public bool IsActive
        {
            get { return m_bIsActive; }
            set { m_bIsActive = value; }
        }

        public bool IsDeleted
        {
            get { return m_bIsDeleted; }
            set { m_bIsDeleted = value; }
        }

        public int Version
        {
            get { return m_iVersion; }
            set { m_iVersion = value; }
        }

        public DateTime CreateDate
        {
            get { return m_dtCreateDate; }
            set { m_dtCreateDate = value; }
        }

        public int CreateBy
        {
            get { return m_iCreateByUserId; }
            set { m_iCreateByUserId = value; }
        }

        public DateTime UpdateDate
        {
            get { return m_dtUpdateDate; }
            set { m_dtUpdateDate = value; }
        }

        public int UpdateBy
        {
            get { return m_iUpdateByUserId; }
            set { m_iUpdateByUserId = value; }
        }

        #endregion // end of Property     
    }
}