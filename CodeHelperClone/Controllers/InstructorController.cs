using Antlr.Runtime.Tree;
using CodeHelperClone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace CodeHelperClone.Controllers
{
    [Authorize(Roles="Instructor")]
    public class InstructorController : Controller
    {
        DBLayer db = new DBLayer();
        // GET: Instructor
        public ActionResult Dashboard()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageDashboard", sp);
            return View(dt);
        }


        public ActionResult InstructorCourse()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@UserID", User.Identity.Name)
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
                new SqlParameter("@course_price",cd.coursePrice),
                new SqlParameter("@UserID", User.Identity.Name)
            };
            int res = db.ExecuteDML("sp_manageCourseData", sp);
            if (res > 0)
            {
                cd.course_thumb.SaveAs(Server.MapPath("/Content/images/courseImage/") + cd.course_thumb.FileName);
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


        public ActionResult CourseVideoSection(int? id)
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
        public ActionResult SaveSectionData(int? course_id, string section_name)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",3),
                new SqlParameter("@course_admin", User.Identity.Name),
                new SqlParameter("@course_id", course_id),
                new SqlParameter("@section_name", section_name)
            };
            int res = db.ExecuteDML("sp_manageCourseData", sp);
            if (res > 0)
            {
                return Json("Section Added",JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Please try after some time.", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult SaveSectionChapter(chapterModel cm)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",5),
                new SqlParameter("@course_id", cm.course_id),
                new SqlParameter("@section_id", cm.section_id),
                new SqlParameter("@chapter_name", cm.chap_name),
                new SqlParameter("@chapter_video", cm.chap_video)

            };
            int res = db.ExecuteDML("sp_manageCourseData", sp);
            if (res > 0)
            {
            return Json("New Chapter is added.");
            }
            else
            {
            return Json("Internal server error.");
            }
        }

        
        public ActionResult ShowRelatedData(int? course_id, int? section_id)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action", 6),
                new SqlParameter("@course_id", course_id),
                new SqlParameter("@section_id", section_id)
            };
            DataTable dt = db.ExecuteSelect("sp_manageCourseData", sp);
            if (dt.Rows.Count > 0)
            {

            List< chapterModel > list = new List< chapterModel >();
            foreach(DataRow data in dt.Rows)
            {
                    chapterModel listItem = new chapterModel();
                    listItem.chap_name = data["chapter_name"].ToString();
                    listItem.chap_video = data["chapter_video"].ToString();
                    list.Add(listItem);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Some Error Occured");
            }
        }
        public ActionResult StudentList()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@inst_id", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_InstructorStuden",sp);
            return View(dt);
        }

        public ActionResult ExportToCsv()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
        new SqlParameter("@action",1),
        new SqlParameter("@inst_id", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_InstructorStuden", sp);
            if (dt.Rows.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Student Name, Course, Course Price, Student Email, Enroll Date");
                List<ExposrtCSVFile> list = new List<ExposrtCSVFile>();
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ExposrtCSVFile()
                    {
                        email = row["student_data"].ToString(),
                        name = row["st_name"].ToString(),
                        courseName = row["course_title"].ToString(),
                        coursePrice = Convert.ToInt32(row["course_price"]) - Convert.ToInt32(row["course_Discount"]),
                        enrollDate = Convert.ToDateTime(row["enrollData"]).ToString("dd-MM-yyyy")

                    });
                }

                foreach (var row in list)
                {
                    stringBuilder.AppendLine($"{row.name}, {row.courseName}, {row.coursePrice}, {row.email}, {row.enrollDate}");
                }

                return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "export.csv");
            }
            else
            {

            return Content("No data to export");
            }
        }

        public ActionResult EditeInstructorProfile() 
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",3),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageInstructor", sp);
            return View(dt.Rows[0]);
        }

        [HttpPost]
        public ActionResult EditeInstructorProfile(RegisterManagementModel rm)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",4),
                new SqlParameter("@name",rm.name),
                new SqlParameter("@email",rm.email),
                new SqlParameter("@mobile",rm.mobile),
                new SqlParameter("@gender",rm.gender),
                new SqlParameter("@summary", rm.summary),
                new SqlParameter("@about_inst", rm.about_Int)
            };

            int res = db.ExecuteDML("sp_manageInstructor", sp);
            if (res > 0)
            {
                Response.Write("<script>alert('Profile updated.');</script>");
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        public ActionResult DeleteAccount() 
        {
            return View();
        }


        public ActionResult RequestForDelete()
        {
            MailMessage message = new MailMessage("mauryaankitt563@gmail.com", User.Identity.Name);
            message.Subject = "User id and password from code helper";
            message.Body = $"Dear Admin,</br> I want to delete my account. It is my humble request to remove my account from your server. </br> Here is my some details which help you in removing precess. <br/> My Email Id : <b>{User.Identity.Name}</b> </b></br> Thank You";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("mauryaankitt563@gmail.com", "vnps zzcp nxib iprs");
            smtp.EnableSsl = true;
            int emailSentCount = 0;
            smtp.Send(message);
            emailSentCount++;
            string alterMessage;
            if(emailSentCount > 0)
            {

                alterMessage = "Your account will be removed within 24 hours.";
            }
            else
            {
                alterMessage = "Please try after some time.";
            }
            return Json(alterMessage);
        }
        [HttpPost]
        public ActionResult MatchPass(string pass)
        {
            SqlParameter[] sp = new SqlParameter[] { 
                new SqlParameter("@action", 5),
                new SqlParameter("@email", User.Identity.Name),
                new SqlParameter("@pass",pass)
            };
            DataTable dt = db.ExecuteSelect("sp_manageInstructor",sp);
            string alertMessage;
            if (dt.Rows.Count > 0)
            {
                alertMessage = "Password matched.";
            }
            else
            {
                alertMessage = "wrong password.";
            }
            return Json(alertMessage);
        }
        [HttpPost]
        public ActionResult changePassword(string pass)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",6),
                new SqlParameter("@email", User.Identity.Name),
                new SqlParameter("@pass", pass)
            };
            int res = db.ExecuteDML("sp_manageInstructor",sp);
            string alertMessage;
            if (res > 0)
            {
                alertMessage = "Password Updated";
            }
            else
            {
                alertMessage = "Some Error Occured. Please try after sometime.";
            }
            return Json(alertMessage);
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous, HttpPost]
        public ActionResult Register(RegisterManagementModel rm)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@name",rm.name),
                new SqlParameter("@email",rm.email),
                new SqlParameter("@pass",rm.password),
                new SqlParameter("@mobile",rm.mobile),
                new SqlParameter("@gender",rm.gender),
                new SqlParameter("@profile",rm.profile_pic.FileName),
                new SqlParameter("@role","Instructor")
            };
            int res = db.ExecuteDML("sp_manageInstructor", sp);
            if (res > 0)
            {
                rm.profile_pic.SaveAs(Server.MapPath("/Content/images/InstructorProfile/") + rm.profile_pic.FileName);
                MailMessage message = new MailMessage("mauryaankitt563@gmail.com", rm.email);
                message.Subject = "User id and password from code helper";
                message.Body = $"Dear {rm.name},</br> Thanks for being register with codehelper.</br> Your Email Id is<b> {rm.email}</b> </b></br> Thank You";
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("mauryaankitt563@gmail.com", "vnps zzcp nxib iprs");
                smtp.EnableSsl = true;
                smtp.Send(message);
                return RedirectToAction("Dashboard");
            }
            else
            {

            return View();
            }
        }


        //instructor orders table

        public ActionResult InstructorOrder()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@inst_email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_instructorOrders", sp);
            return View(dt);
        }


        //instructor extra discount section

        public ActionResult supportTickets()
        {
            // All table data
            SqlParameter[] allDatas = new SqlParameter[]
            {
                new SqlParameter("@action",6),
                new SqlParameter("@admin_email", User.Identity.Name)
            };
            DataTable allData = db.ExecuteSelect("sp_ExtraDiscount", allDatas);


            // Total Ticket Data
            SqlParameter[] TotalData = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable TotalTicket = db.ExecuteSelect("sp_manageExtraDiscountData", TotalData);


            //  Only Opened Ticket Data
            SqlParameter[] Opened = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable OpenedTicket = db.ExecuteSelect("sp_manageExtraDiscountData", Opened);



            // Only Closed Ticket Data
            SqlParameter[] Closed = new SqlParameter[]
            {
                new SqlParameter("@action",3),
                new SqlParameter("@email", User.Identity.Name)
            };
            DataTable ClosedTicket = db.ExecuteSelect("sp_manageExtraDiscountData", Closed);


            DataSet ds = new DataSet();
            ds.Tables.Add(allData);
            ds.Tables.Add(TotalTicket);
            ds.Tables.Add(OpenedTicket);
            ds.Tables.Add(ClosedTicket);

            return View(ds);
        }

        public ActionResult addTickets()
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",1),
                new SqlParameter("@admin_email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_ExtraDiscount", sp);
            return View(dt);
        }
        [HttpPost]
        public ActionResult addTickets(ExtraDiscountModel em)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",4),
                new SqlParameter("@admin_email", User.Identity.Name),
                new SqlParameter("@email",em.userEmail),
                new SqlParameter("@courseid", em.course),
                new SqlParameter("@course_fee",em.courseFee),
                new SqlParameter("@course_expiry", em.courseExpiry),
                new SqlParameter("@extra_discount", em.extraDiscount),
                new SqlParameter("@valid_discount", em.dis_validity)
            };


            int res = db.ExecuteDML("sp_ExtraDiscount", sp);
            if (res==1)
            {
                ViewBag.alertMessage = "Discount Enrolled for stuednt";
                ViewBag.alertTyper = "success";
                MailMessage message = new MailMessage("mauryaankitt563@gmail.com", em.userEmail);
                message.Subject = "Bonus Discount";
                message.Body = $"Dear Student,</br> We have give you an special discount offer. This offer is valid till {em.dis_validity}.<br/> Avail this offer before its expiry. ";
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("mauryaankitt563@gmail.com", "vnps zzcp nxib iprs");
                smtp.EnableSsl = true;
                smtp.Send(message);

                return RedirectToAction("addTickets");
            }
            else
            {
                ViewBag.alertMessage = "Some Error Occured. Please try after sometime.";
                ViewBag.alertTyper = "danger";
                return View();
            }
        }

        public ActionResult CheckValidUser(string st_email, int course)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",5),
                new SqlParameter("@courseid", course),
                new SqlParameter("@email",st_email)
            };
            var res = db.ExecuteScalar("sp_ExtraDiscount", sp);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckCourseDetail(int courseid)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",3),
                new SqlParameter("@courseid", courseid),
                new SqlParameter("@admin_email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_ExtraDiscount", sp);
            if (dt.Rows.Count > 0)
            {
                List<string> lists = new List<string>();
                lists.Add(Convert.ToDateTime(dt.Rows[0]["course_ExpireDate"]).ToShortDateString().ToString());
                lists.Add(dt.Rows[0]["course_price"].ToString());
                return Json(lists, JsonRequestBehavior.AllowGet);
            }
            else
            {
            return Json("Some Error Occured");

            }
        }
        //checking user for their extra discount
        public ActionResult checkUser(string email)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",2),
                new SqlParameter("@email", email)
            };
            var msg = db.ExecuteScalar("sp_ExtraDiscount", sp);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FetchLogedinData(string userId)
            {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@action",7),
                new SqlParameter("@email", userId)
            };
            DataTable dt = db.ExecuteSelect("sp_manageInstructor", sp);
            if (dt.Rows.Count > 0)
            {
                List<string> list = new List<string>();
                foreach(DataRow data in dt.Rows)
                {
                    list.Add(data["profile_pic"].ToString());
                    list.Add(data["name"].ToString());
                }
            return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("<script>alert('User Not Found.')</script>");
            }
        }



        public JsonResult getEarningChartData()
        {
            SqlParameter[] sp = new SqlParameter[] { 
            new SqlParameter("@action",2),
            new SqlParameter("@email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageDashboard", sp);
            if(dt.Rows.Count > 0)
            {
                List<int> list = new List<int>();
                foreach(DataRow data in dt.Rows)
                {
                    list.Add(Convert.ToInt32(data["Income"]));
                    list.Add(Convert.ToInt32(data["income_month"]));
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
            return Json("Some Error Occured", JsonRequestBehavior.AllowGet);

            }

        }

        
        public JsonResult GetOrderChartData()
        {
            SqlParameter[] sp = new SqlParameter[] { 
            new SqlParameter("@action",3),
            new SqlParameter("@email", User.Identity.Name)
            };
            DataTable dt = db.ExecuteSelect("sp_manageDashboard", sp);
            if(dt.Rows.Count > 0)
            {
                List<int> list = new List<int>();
                foreach(DataRow data in dt.Rows)
                {
                    list.Add(Convert.ToInt32(data["totalOrder"]));
                    list.Add(Convert.ToInt32(data["monthName"]));
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
            return Json("Some Error Occured", JsonRequestBehavior.AllowGet);

            }

        }

        

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("dashboard");
        }

    }
}