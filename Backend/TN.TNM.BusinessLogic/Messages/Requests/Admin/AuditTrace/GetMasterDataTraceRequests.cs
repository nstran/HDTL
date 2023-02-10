using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.AuditTrace
{
    public class GetMasterDataTraceRequests : BaseRequest<GetMasterDataTraceParameter>

    {
        public override GetMasterDataTraceParameter ToParameter()
        {
            return new GetMasterDataTraceParameter()
            {
                UserId = UserId
            };
        }
    }
}
