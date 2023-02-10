using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataBarchartFollowMonthResult : BaseResult
    {
        public List<dynamic> MonthOrderList { get; set; }
        public List<dynamic> MonthOrderAndRequestList { get; set; }
    }
}
