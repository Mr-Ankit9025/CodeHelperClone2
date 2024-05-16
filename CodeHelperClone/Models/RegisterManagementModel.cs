using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeHelperClone.Models
{
    public class RegisterManagementModel
    {
        public string name{ get; set; }
        public string email { get; set; }
        public long mobile { get; set; }
        public string gender { get; set; }
        public HttpPostedFileBase profile_pic { get; set; }
        public string password { get; set; }
        public string remember { get; set; }
        public string summary { get; set; }
        
        [AllowHtml]
        public string about_Int { get; set; }

    }   
}