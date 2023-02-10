using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class RevenueStatisticModel
    {
        public decimal RevenueOfDay { get; set; }
        public decimal RevenueOfWeek { get; set; }
        public decimal RevenueOfMonth { get; set; }
        public decimal RevenueOfQuarter { get; set; }
        public decimal RevenueOfYear { get; set; }
        public decimal RevenueOfYesterday { get; set; }
        public decimal RevenueOfLastWeek { get; set; }
        public decimal RevenueOfLastMonth { get; set; }
        public decimal RevenueOfLastQuarter { get; set; }
        public decimal RevenueOfLastYear { get; set; }
        public decimal RevenueWaitPayment { get; set; }
    }
}
