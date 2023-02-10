using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataListSearchProductionOrderRequest : BaseRequest<GetMasterDataListSearchProductionOrderParameter>
    {
        public override GetMasterDataListSearchProductionOrderParameter ToParameter()
        {
            return new GetMasterDataListSearchProductionOrderParameter()
            {
                UserId = UserId
            };
        }
    }
}
