using System.Web.Mvc;
using Azur.Web.UI.Helper;
using Azur.Web.UI.Models;

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
    public class UserController : Controller
    {
        #region Method

        // GET: User
        public ActionResult Index()
        {
            DataAccess oDataAccess = new DataAccess();

            ModelState.Clear();
            return View(oDataAccess.Get());
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(UserModel p_oUserModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DataAccess oDataAccess = new DataAccess();

                    if (oDataAccess.Add(p_oUserModel))
                    {
                        ModelState.Clear();
                        ViewBag.Message = "User Added Successfully!";
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "User Added Aborted!";
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int p_iUserId, int p_iVersion)
        {
            DataAccess oDataAccess = new DataAccess();

            return View(oDataAccess.Get().Find(Models => Models.UserId == p_iUserId
                && Models.Version == p_iVersion));
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int p_iUserId, int p_iVersion, UserModel p_oUserModel)
        {
            try
            {
                DataAccess oDataAccess = new DataAccess();
                oDataAccess.Update(p_oUserModel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion // end of Method 
    }
}