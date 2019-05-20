using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        protected int m_iStorageId = -99;
        protected string m_strImageName = string.Empty;
        protected string m_strImageUri = string.Empty;
        protected string m_strThumbnailUri = string.Empty;
        protected string m_strCaption = string.Empty;
        protected string m_strResponse = string.Empty;

        #endregion // end of Variable 

        #region Constructor

        public StorageModel()
        {
            m_iStorageId = -99;
        }

        public StorageModel(int p_iStorageId)
        {
            m_iStorageId = p_iStorageId;
        }

        public StorageModel(
            string p_strImageName,
            string p_strCaption,
            string p_strImageUri,
            string p_strThumbnailUri,
            string p_strResponse)
        {
            m_strImageName = p_strImageName;
            m_strCaption = p_strCaption;
            m_strImageUri = p_strImageUri;
            m_strThumbnailUri = p_strThumbnailUri;
            m_strResponse = p_strResponse;
        }

        #endregion // end of Constructor 

        #region Property

        [Display(Name = "Storage Id")]
        public int StorageId
        {
            get { return m_iStorageId; }
            set { m_iStorageId = value; }
        }

        [Required(ErrorMessage = "Image Name is Required!")]
        public string ImageName
        {
            get { return m_strImageName; }
            set { m_strImageName = value; }
        }

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

        [Required(ErrorMessage = "Response is Required!")]
        public string Response
        {
            get { return m_strResponse; }
            set { m_strResponse = value; }
        }

        #endregion // end of Property 
    }

    #region Json Response Date

    public class Detail
    {
        public List<object> celebrities { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public double score { get; set; }
        public Detail detail { get; set; }
    }

    public class Caption
    {
        public string text { get; set; }
        public double confidence { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public List<Caption> captions { get; set; }
    }

    public class Color
    {
        public string dominantColorForeground { get; set; }
        public string dominantColorBackground { get; set; }
        public List<string> dominantColors { get; set; }
        public string accentColor { get; set; }
        public bool isBwImg { get; set; }
    }

    public class Metadata
    {
        public int height { get; set; }
        public int width { get; set; }
        public string format { get; set; }
    }

    public class ImageInfo
    {
        public List<Category> categories { get; set; }
        public Description description { get; set; }
        public Color color { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }

    #endregion // end of Json Response Date 

}