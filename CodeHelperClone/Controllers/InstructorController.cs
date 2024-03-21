using CodeHelperClone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CodeHelperClone.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        DBLayer db = new DBLayer();
        // GET: Instructor
        public ActionResult Dashboard()
        {
            return View();
        }


        public ActionResult InstructorCourse()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2)
            };
            DataTable dt = db.ExecuteSelect("sp_manageCourseData", sp);
            return View(dt);
        }


        public ActionResult InstructorChangePass()
        {
            return View();
        }

        public ActionResult InstructorAddCourse()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2)
            };
            DataTable dt = db.ExecuteSelect("sp_manageAdminData", sp);
            return View(dt);
        }

        public ActionResult CourseVideoSection(int?id)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",4),
                new SqlParameter("@course_id", id),
                new SqlParameter("@course_admin",User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageCourseData", sp);
            return View(dt);
        }
        [HttpPost]
        public ActionResult SaveSectionData(int?course_id, string section_name)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",3),
                new SqlParameter("@course_admin", User.Identity.Name),
                new SqlParameter("@course_id", course_id),
                new SqlParameter("@section_name", section_name)
            };
            int res = db.ExecuteDML("sp_manageCourseData",sp);
            if (res > 0)
            {
                return Json("Section Added");
            }
            else
            {
                return Json("Please try after some time.");
            }
        }
        [HttpPost]
        public ActionResult InstructorAddCourse(courseData cd)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@course_title",cd.course_title),
                new SqlParameter("@course_cat",cd.course_cat),
                new SqlParameter("@course_level",cd.course_level),
                new SqlParameter("@course_disc",cd.course_desc),
                new SqlParameter("@course_expireDate",cd.course_expireDate),
                new SqlParameter("@course_cover_image",cd.course_thumb.FileName),
                new SqlParameter("@videoUrl",cd.video_url),
                new SqlParameter("@course_req",cd.html),
                new SqlParameter("@course_price",cd.coursePrice)
            };
            int res = db.ExecuteDML("sp_manageCourseData",sp);
            if (res > 0)
            {
                cd.course_thumb.SaveAs(Server.MapPath("/Content/images/courseImage/")+cd.course_thumb.FileName);
                ViewBag.AlertMessage = "Data added successfully.";
                ViewBag.AllertType = "success";
            return View();
            }
            else
            {
                ViewBag.AlertMessage = "Some Error Occured while adding your data. Please Try after sometime.";
                ViewBag.AllertType = "danger";
                return View();
            }
        }

        public ActionResult CourseDetail()
        {
            return View();
        }
        public ActionResult EditeInstructorProfile() 
        {
            return View();
        }

        public ActionResult DeleteAccount() 
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous,HttpPost]
        public ActionResult Login(string email, string pass)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@email",email),
                new SqlParameter("@pass",pass)
            };
           DataTable dt = db.ExecuteSelect("sp_manageInstructor",sp);
            if(dt.Rows.Count > 0)
            {
                FormsAuthentication.SetAuthCookie(email, false);
                return RedirectToAction("Dashboard");
            }
            return View(dt);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login");
        }

    }
}