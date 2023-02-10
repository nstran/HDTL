using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetReportManuFactureByMonthResult : BaseResult
    {
        public List<Models.Manufacture.ReportManuFactureByMonthModel> ListReportManuFactureByMonth { get; set; }
        public Models.Manufacture.SumaryReportManuFactureByMonthModel SumaryReportManuFactureByMonth { get; set; }
    }
}
