using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class RevenueStatisticServicePacketModel
    {
        public string ProductCategoryName { get; set; }

        public List<decimal?> ListAmount { get; set; }

        public string ServicePacketName { get; set; }

    }
}
