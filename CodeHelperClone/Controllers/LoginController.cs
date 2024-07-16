using CodeHelperClone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GoogleAuthentication.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CodeHelperClone.Controllers
{
    public class LoginController : Controller
    {
        DBLayer db= new DBLayer();
        public ActionResult Login()
        {
            var clientId = "211310085751-611umg73771eeoaoic1cc8ld66afvjbg.apps.googleusercontent.com";
            var url = "https://localhost:44372/login/googleLogin";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.response = response;
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string pass)
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
                if (dt.Rows[0]["Roles"].ToString() == "Instructor")
                {

                    FormsAuthentication.SetAuthCookie(email, false);
                    return RedirectToAction("Dashboard", "Instructor");
                }
                else if (dt.Rows[0]["Roles"].ToString() == "Student")
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return RedirectToAction("Index", "Home");
                }
                else if (dt.Rows[0]["Roles"].ToString() == "Admin")
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }


        public async Task<ActionResult> googleLogin(string code)
        {
            var clientID = "211310085751-611umg73771eeoaoic1cc8ld66afvjbg.apps.googleusercontent.com";
            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)+"/login/googleLogin";
            var clientSecrete = "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";
            var token = await GoogleAuth.GetAuthAccessToken(code,clientID, clientSecrete, url);
            var profile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            var json = JObject.Parse(profile);
            if (Convert.ToBoolean(json["verified_email"]))
            {
                SqlParameter[] sp = new SqlParameter[]
                {
                    new SqlParameter("@action",1),
                    new SqlParameter("@email",json["email"].ToString())
                };
                DataTable dt = db.ExecuteSelect("sp_manageLoginSystem", sp);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Role"].ToString() == "Instructor")
                    {
                        FormsAuthentication.SetAuthCookie(json["email"].ToString(), false);
                        return RedirectToAction("Dashboard", "Instructor");
                    }
                    else if (dt.Rows[0]["Role"].ToString() == "Student")
                    {
                        FormsAuthentication.SetAuthCookie(json["email"].ToString(), false);
                        return RedirectToAction("MyCourse", "Home");
                    }
                    else
                    {
                        return Content("<script>alert('User is not in specified role'); location.href='/login/login';</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('It Look Like User is not registered with us. Please register yourself. Thank you!'); location.href='/login/login';</script>");
                }
            }
            else
            {
                return Content("<script>alert('Email is verified. please try again later'); location.href='/login/login';</script>");
            }
        }


    }
}
