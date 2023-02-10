using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeRevenueStatisticEmployeeDashboardResult : BaseResult
    {
        public List<RevenueStatisticEmployeeModel> ListRevenueStatisticEmployeeModel { get; set; }
    }
}
