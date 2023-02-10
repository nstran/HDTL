using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Dashboard
{
    public class TakeRevenueStatisticDashboardByFilterParameter : BaseParameter
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Count { get; set; }
    }
}
