using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeRevenueStatisticServicePacketDashboardResult : BaseResult
    {
        public List<RevenueStatisticServicePacketModel> ListRevenueStatisticServicePacketModel { get; set; }

        public List<RevenueStatisticServicePacketModel> ListRevenueStatisticServicePacketModelByServicePacket { get; set; }

    }
}
