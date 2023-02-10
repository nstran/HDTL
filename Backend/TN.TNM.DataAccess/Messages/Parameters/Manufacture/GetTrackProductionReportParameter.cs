using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Manufacture
{
    public class GetTrackProductionReportParameter: BaseParameter
    {
        public string OrganizationCode { get; set; }
        public List<Guid?> ListProductionOrderId { get; set; }
    }
}
