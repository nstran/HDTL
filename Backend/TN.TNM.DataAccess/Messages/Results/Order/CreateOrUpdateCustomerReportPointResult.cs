using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class CreateOrUpdateCustomerReportPointResult: BaseResult
    {
        public Guid ReportPointId { get; set; }
    }
}
