using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHelperClone.Models
{
    public class ExtraDiscountModel
    {
        public int ID { get; set; }
        public string userEmail { get; set; }
        public int course { get; set; }
        public int courseFee { get; set; }
        public DateTime courseExpiry { get; set; }
        public int extraDiscount { get; set; }
        public DateTime dis_validity { get; set; }
    }
}