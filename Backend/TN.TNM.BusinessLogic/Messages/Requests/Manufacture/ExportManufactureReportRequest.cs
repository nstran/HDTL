using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class ExportManufactureReportRequest : BaseRequest<ExportManufactureReportParameter>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override ExportManufactureReportParameter ToParameter()
        {
            return new ExportManufactureReportParameter()
            {
                FromDate = FromDate,
                ToDate = ToDate
            };
        }
    }
}
