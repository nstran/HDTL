using System.Collections.Generic;
using TN.TNM.DataAccess.Models.AuditTrace;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace
{
    public class GetMasterDataTraceResult : BaseResult
    {
        // public List<LoginTraceEntityModel> ListLoginTrace { get; set; }
        
        //public List<TraceEntityModel> ListAuditTrace { get; set; }

        public List<EmployeeEntityModel> ListEmp { get; set; }

        public  List<UserEntityModel> ListUser { get; set; }
        
    }
}
