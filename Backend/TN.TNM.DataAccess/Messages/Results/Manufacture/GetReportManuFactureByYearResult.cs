using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetReportManuFactureByYearResult: BaseResult
    {
        public List<Models.Manufacture.ReportManuFactureByYearModel> ListReportManuFactureByYear { get; set; }
        public Models.Manufacture.SumaryReportManuFactureByYearModel SumaryReportManuFactureByYear { get; set; }
    }
}
