using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetReportManuFactureByMonthResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.ReportManuFactureByMonthModel> ListReportManuFactureByMonth { get; set; }
        public DataAccess.Models.Manufacture.SumaryReportManuFactureByMonthModel SumaryReportManuFactureByMonth { get; set; }
    }
}
