using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetReportManuFactureByYearResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.ReportManuFactureByYearModel> ListReportManuFactureByYear { get; set; }
        public DataAccess.Models.Manufacture.SumaryReportManuFactureByYearModel SumaryReportManuFactureByYear { get; set; }
    }
}
