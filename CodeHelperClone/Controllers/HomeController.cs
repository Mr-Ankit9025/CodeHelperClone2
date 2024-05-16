using CodeHelperClone.Models;
using GoogleAuthentication.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Razorpay.Api;
using System.Configuration;

namespace CodeHelperClone.Controllers
{
   
    public class HomeController : Controller
    {
        DBLayer db = new DBLayer();
        // GET: Home
        public ActionResult Index()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2)
        };
            DataTable dt = db.ExecuteSelect("sp_manageAdminData",sp);
            SqlParameter[] sp2 = new SqlParameter[]
            {
                new SqlParameter("@action",1)
            };
            DataTable dt2 = db.ExecuteSelect("sp_selectCourseData", sp2);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            return View(ds);
        }

        public ActionResult CourseDetail(int?id)
        {
            if (id.HasValue)
            {
                SqlParameter[] sp = new SqlParameter[]
                {
                    new SqlParameter("@action", 2),
                    new SqlParameter("@id",id)
                };
                DataTable dt = db.ExecuteSelect("sp_selectCourseData", sp);

                SqlParameter[] sp2 = new SqlParameter[]
                {
                    new SqlParameter("@action",7),
                    new SqlParameter("@course_id",id)
                };
                DataTable dt2 = db.ExecuteSelect("sp_manageCourseData", sp2);


                SqlParameter[] checkEnroll = new SqlParameter[]
                {
                    new SqlParameter("@action",8 ),
                    new SqlParameter("@course_id", id),
                    new SqlParameter("@UserID", User.Identity.Name)
                };
                DataTable checkData = db.ExecuteSelect("sp_manageCourseData", checkEnroll);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                ds.Tables.Add(dt2);
                ds.Tables.Add(checkData);
            return View(ds);
            }
            else
            {
                return RedirectToAction("index");
            }

        }



        public ActionResult ShowSelectedChapter(int courseId, int sectionId)
        {
            SqlParameter[] sp= new SqlParameter[]
            {
                new SqlParameter("@action", 6),
                new SqlParameter("@course_id", courseId),
                new SqlParameter("@section_id", sectionId)

            };
            DataTable dt = db.ExecuteSelect("sp_manageCourseData", sp);
            if(dt.Rows.Count > 0)
            {
                List<chapterModel> lists = new List<chapterModel>();
                foreach (DataRow dr in dt.Rows)
                {
                    lists.Add(new chapterModel()
                    {
                        chap_name = dr["chapter_name"].ToString(),
                        chap_video = dr["chapter_video"].ToString()
                    });
                }
            return Json(lists, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("<script>alert('No Data found.')</script>");
                   
            }
        }

        [Authorize(Roles="Student")]
        public ActionResult MyCourse()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@student_data",User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageCourseEnroll",sp);
            return View(dt);
        }

        [Authorize(Roles ="Student")]
        public ActionResult Play_Course_Video( int id)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@courseId", id),
                new SqlParameter("@studentId", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_getCourseVideo", sp);
            return View(dt);
        }

        [Authorize(Roles ="Student")]
        public ActionResult EnrollForCourse(int id)
        {

            SqlParameter[] userDetail = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@st_email",User.Identity.Name),
            };
            DataTable userData = db.ExecuteSelect("sp_ManageStudent", userDetail);

            SqlParameter[] courseDetail = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@courseId",id),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable CourseData = db.ExecuteSelect("sp_ApplyExtraDiscount", courseDetail);

            DataSet ds = new DataSet();
            ds.Tables.Add(userData);
            ds.Tables.Add(CourseData);

            int totalamount = Convert.ToInt32(CourseData.Rows[0]["course_price"]) - Convert.ToInt32(CourseData.Rows[0]["Discount"]);
            var key = ConfigurationManager.AppSettings["RazorPayKey"].ToString();
            var secret = ConfigurationManager.AppSettings["RazorpaySecret"].ToString();
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(totalamount) * 100);
            options.Add("currency", "INR");
            Order order = client.Order.Create(options);
            ViewBag.OrderId = order["id"].ToString();
            return View(ds);
        }

        public ActionResult ManagePayment(string paymentId, string orderId, string signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", paymentId);
            attributes.Add("razorpay_order_id", orderId);
            attributes.Add("razorpay_signature", signature);
            try
            {
                Utils.verifyPaymentSignature(attributes);
                return Json("Payment Successfull.", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content("<script>alert('Payment not successfull.'); location.href='#';</script>");
            }
        }
        [HttpPost]
        public ActionResult CourseEnrollData(int adminId, int courseId, string st_Id, int course_dis, int coursePrice)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@course_Admin", adminId),
                new SqlParameter("@course_id", courseId),
                new SqlParameter("@student_data", st_Id),
                new SqlParameter("@course_price", coursePrice),

                new SqlParameter("@course_Discount", course_dis)
            };
            var res = db.ExecuteScalar("sp_manageCourseEnroll", sp);
            if (res.Equals(1))
            {
            return Json("Thank you for course Enrollment", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Some Error Occured", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StudentLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StudentLogin(string email, string pass)
        {
            SqlParameter[] sp = new SqlParameter[]
                {
                new SqlParameter("@action",1),
                new SqlParameter("@email",email),
                new SqlParameter("@pass",pass)
                };
                DataTable dt = db.ExecuteSelect("sp_manageInstructor", sp);
                if (dt.Rows.Count > 0)
                {
                   FormsAuthentication.SetAuthCookie(email, false);
                HttpCookie userInfo = new HttpCookie("userInfo");
                userInfo["profile"] = dt.Rows[0]["profile_pic"].ToString();
                userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                Response.Cookies.Add(userInfo);
                return RedirectToAction("index");
                }
                return View();
            }
        public ActionResult StudentRegister() {
            return View();

        }
        [HttpPost]
        public ActionResult StudentRegister(RegisterManagementModel rm)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@st_name",rm.name),
                new SqlParameter("@st_email",rm.email),
                new SqlParameter("@st_pass",rm.password),
                new SqlParameter("@st_mobile",rm.mobile),
                new SqlParameter("@st_gender",rm.gender),
                new SqlParameter("@st_profile",rm.profile_pic.FileName),
                new SqlParameter("@role","Student")
            };
            int res = db.ExecuteDML("sp_ManageStudent", sp);
            if (res > 0)
            {
                rm.profile_pic.SaveAs(Server.MapPath("/Content/images/StudentProfile/") + rm.profile_pic.FileName);
                MailMessage message = new MailMessage("mauryaankitt563@gmail.com", rm.email);
                message.Subject = "Thank you For Registration";
                message.Body = $"Dear {rm.name},</br> Thanks for being register with codehelper.</br> Your Email Id is<b> {rm.email}</b> </b></br> Thank You";
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("mauryaankitt563@gmail.com", "vnps zzcp nxib iprs");
                smtp.EnableSsl = true;
                smtp.Send(message);
                return RedirectToAction("StudentLogin");
            }
            else
            {

                return View();
            }
        }

        // fetch data for authorised user.
        
        public ActionResult fetchAuthorisedUserData(String userID)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@st_email",userID)
            };
            DataTable dt = db.ExecuteSelect("sp_ManageStudent", sp);
            if(dt.Rows.Count > 0)
            {
                List<String> list = new List<String>();
                foreach(DataRow dr in dt.Rows)
                {
                    list.Add(dr["st_name"].ToString());
                    list.Add(dr["st_profile"].ToString().Trim());
                }
            return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index");
            }
        }

        // logout system started

        public ActionResult Logout()
        {
            if (User.IsInRole("Student"))
            {
            FormsAuthentication.SignOut(); 
            }
            return RedirectToAction("Index");
        }


        // errro pages comes here

        public ActionResult Error404()
        {
            return View();
        }

    }
}
