using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

#region Trademark

/**
 *  This software, all associated documentation, and all copies are CONFIDENTIAL INFORMATION of Kalpawreska Teknologi Indonesia
 *  http://www.fwahyudianto.id
 *  ® Wahyudianto, Fajar
 *  Email 	: fwahyudi06@gmail.com
 */

#endregion // end of Trademark

namespace Azur.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        #region Method

        /// <summary>
        /// Dashboard Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get: About Page
        /// </summary>
        /// <returns>
        ///     <message>
        ///         Your application description page
        ///     </message>
        /// </returns>
        public ActionResult About()
        {


            return View();
        }

        /// <summary>
        /// Get: Contact Page
        /// </summary>
        /// <returns>
        ///     <message>
        ///         Your contact page
        ///     </message>
        public ActionResult Contact()
        {
            return View();
        }

        #endregion // end of Method 
    }
}