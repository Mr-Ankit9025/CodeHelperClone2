using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeHelperClone.Models
{
    public class courseData
    {
        public int id { get; set; }
        public string course_title { get; set; }
        public int course_cat { get; set; }
        public DateTime course_expireDate { get; set; }
        public string course_level { get; set; }
        [AllowHtml]
        public string course_desc { get; set; }
        public HttpPostedFileBase course_thumb { get; set; }
        public string video_url { get; set; }
        public string html { get; set; }
        public int coursePrice { get; set; }

    }


    public class chapterModel
    {
        public int id { get; set; }
        public string chap_name { get; set; }
        public string chap_video { get; set; }
        public int course_id { get; set; }
        public int section_id { get; set; }

    }
}