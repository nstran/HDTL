using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class GetDataBarchartFollowMonthParameter : BaseParameter
    {
        public DateTime Date { get; set; }
        public int Month { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
