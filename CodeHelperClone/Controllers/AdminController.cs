using CodeHelperClone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeHelperClone.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        DBLayer db = new DBLayer();
        // GET: Admin
        public ActionResult Index()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2)
            };

            DataTable dt = db.ExecuteSelect("sp_manageAdminData", sp);

            return View(dt);
        }
        [HttpPost]
        public ActionResult Index(string courseCategoryName,HttpPostedFileBase categoryIcon)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action", 1),
                new SqlParameter("@cat_name", courseCategoryName),
                new SqlParameter("@cat_icon", categoryIcon.FileName)
            };

            int res = db.ExecuteDML("sp_manageAdminData", sp);
            if(res > 0)
            {
                categoryIcon.SaveAs(Server.MapPath("/content/images/CategoryImages/") +categoryIcon.FileName);
                Response.Write("<script>alert('Category Added.')</script>");
                return RedirectToAction("index");
            }
            else
            {
                Response.Write("<script>alert('Please Wait for a while.')</script>");

            return View();
            }
        }

    }
}