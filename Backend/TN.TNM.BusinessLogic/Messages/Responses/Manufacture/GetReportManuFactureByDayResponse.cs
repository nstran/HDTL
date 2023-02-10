using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Manufacture
{
    public class GetReportManuFactureByDayResponse: BaseResponse
    {
        public List<DataAccess.Models.Manufacture.ReportManuFactureByDayModel> ListReportManuFactureByDay { get; set; }
        public DataAccess.Models.Manufacture.SumaryReportManuFactureByDayModel SumaryReportManuFactureByDay { get; set; }
        //public List<DataAccess.Models.Manufacture.ReportManuFactureByDayModel> ListReportManuFactureByDayRemain { get; set; }
        //public DataAccess.Models.Manufacture.SumaryReportManuFactureByDayModel SumaryReportManuFactureByDayRemain { get; set; }
    }
}
