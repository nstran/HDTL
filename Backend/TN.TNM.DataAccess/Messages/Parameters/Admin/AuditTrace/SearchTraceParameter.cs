using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace
{
    public class SearchTraceParameter : BaseParameter
    {
        public List<Guid?> ListUserId { get; set; }
        
        public List<Guid?> ListEmployeeId { get; set; }
        
        public List<int> ListStatus { get; set; }

        public List<string> ListActionName { get; set; }

        public List<string> ListObjectType { get; set; }

        public DateTime? SearchDate { get; set; }

        public bool IsSelectedLoginAudit { get; set; }

        public bool IsSelectedAuditTrace { get; set; }

        public int PageSize { get; set; }
        
        public int PageIndex { get; set; }
    }
}
