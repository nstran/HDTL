using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.AuditTrace
{
    public class SearchTraceResponses : BaseResponse
    {
        public List<LoginTraceModel> ListLoginTrace { get; set; }
        
        public List<TraceModel> ListAuditTrace { get; set; }

        public int TotalRecordsLoginTrace { get; set; }

        public int TotalRecordsAuditTrace { get; set; }
    }
}
