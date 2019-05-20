using System.ComponentModel.DataAnnotations;

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
    public class StorageModel
    {
        #region Variable

        protected string m_strImageUri = string.Empty;
        protected string m_strThumbnailUri = string.Empty;
        protected string m_strCaption = string.Empty;

        #endregion // end of Variable 

        #region Property

        [Required(ErrorMessage = "Image Uri is Required!")]
        public string ImageUri
        {
            get { return m_strImageUri; }
            set { m_strImageUri = value; }
        }

        [Required(ErrorMessage = "Thumbnail Uri is Required!")]
        public string ThumbnailUri
        {
            get { return m_strThumbnailUri; }
            set { m_strThumbnailUri = value; }
        }

        [Required(ErrorMessage = "Caption is Required!")]
        public string Caption
        {
            get { return m_strCaption; }
            set { m_strCaption = value; }
        }

        #endregion // end of Property 
    }
}