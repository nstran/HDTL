using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.AuditTrace
{
    public class GetMasterDataTraceResponses : BaseResponse
    {
        // public List<LoginTraceModel> ListLoginTrace { get; set; }
        
        // public List<TraceModel> ListAuditTrace { get; set; }

        public List<EmployeeModel> ListEmp { get; set; }

        public  List<UserModel> ListUser { get; set; }
    }
}
