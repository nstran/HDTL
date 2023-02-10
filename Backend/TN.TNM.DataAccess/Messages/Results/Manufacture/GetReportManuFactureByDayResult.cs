using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetReportManuFactureByDayResult: BaseResult
    {
        public List<Models.Manufacture.ReportManuFactureByDayModel> ListReportManuFactureByDay { get; set; }
        public Models.Manufacture.SumaryReportManuFactureByDayModel SumaryReportManuFactureByDay { get; set; }
        //public List<Models.Manufacture.ReportManuFactureByDayModel> ListReportManuFactureByDayRemain { get; set; }
        //public Models.Manufacture.SumaryReportManuFactureByDayModel SumaryReportManuFactureByDayRemain { get; set; }
    }
}
