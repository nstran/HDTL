using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetMasterDataAddProductionOrderDialogRequest : BaseRequest<GetMasterDataAddProductionOrderDialogParameter>
    {
        public List<Guid> ListIgnore { get; set; }
        public override GetMasterDataAddProductionOrderDialogParameter ToParameter()
        {
            return new GetMasterDataAddProductionOrderDialogParameter()
            {
                UserId = UserId,
                ListIgnore = ListIgnore
            };
        }
    }
}
