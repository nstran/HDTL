using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeRevenueStatisticWaitPaymentDashboardResult : BaseResult
    {
        public List<RevenueStatisticWaitPaymentModel> ListRevenueStatisticWaitPaymentModel { get; set; }

        //public List<RevenueStatisticWaitPaymentModel> listRevenueStatisticWaitPaymentByServicePacketId { get; set; }

    }
}
