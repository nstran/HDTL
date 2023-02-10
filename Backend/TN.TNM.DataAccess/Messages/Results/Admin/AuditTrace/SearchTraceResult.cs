using System.Collections.Generic;
using TN.TNM.DataAccess.Models.AuditTrace;

namespace TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace
{
    public class SearchTraceResult : BaseResult
    {
        public List<LoginTraceEntityModel> ListLoginTrace { get; set; }
        
        public List<TraceEntityModel> ListAuditTrace { get; set; }

        public int TotalRecordsLoginTrace { get; set; }

        public int TotalRecordsAuditTrace { get; set; }
    }
}
