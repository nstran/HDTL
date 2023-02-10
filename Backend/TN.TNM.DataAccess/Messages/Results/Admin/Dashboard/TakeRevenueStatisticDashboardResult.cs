using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeRevenueStatisticDashboardResult : BaseResult
    {
        public RevenueStatisticModel RevenueStatisticModel { get; set; }
    }
}
