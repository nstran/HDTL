using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class RevenueStatisticWaitPaymentModel
    {
        public string ProductCategoryName { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Percent { get; set; }

        public decimal? PercentOfServicePacket { get; set; }

        public string ServicePacketName { get; set; }

        public List<RevenueStatisticWaitPaymentModel> ListRevenueStatisticWaitPaymentModel { get; set; }
    }
}
