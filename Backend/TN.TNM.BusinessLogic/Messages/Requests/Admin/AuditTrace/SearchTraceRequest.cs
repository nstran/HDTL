using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.AuditTrace
{
    public class SearchTraceRequest : BaseRequest<SearchTraceParameter>
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

        public override SearchTraceParameter ToParameter()
        {
            return new SearchTraceParameter()
            {
                ListUserId = ListUserId,
                ListEmployeeId = ListEmployeeId,
                ListActionName = ListActionName,
                ListObjectType = ListObjectType,
                ListStatus = ListStatus,
                SearchDate = SearchDate,
                IsSelectedLoginAudit = IsSelectedLoginAudit,
                IsSelectedAuditTrace = IsSelectedAuditTrace,
                PageSize = PageSize,
                PageIndex = PageIndex
            };
        }
    }
}
