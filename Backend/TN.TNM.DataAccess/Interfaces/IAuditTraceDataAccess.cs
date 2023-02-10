using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin;
using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;
using TN.TNM.DataAccess.Messages.Results.Admin;
using TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IAuditTraceDataAccess
    {
        void Trace(string actionName, string objectName, string description, Guid createById);

        GetMasterDataTraceResult GetMasterDataTrace(GetMasterDataTraceParameter parameter);

        SearchTraceResult SearchTrace(SearchTraceParameter parameter);
    }
}
