using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;

namespace TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh
{
    public class GetMasterDataSearchQuyTrinhRequest : BaseRequest<GetMasterDataSearchQuyTrinhParameter>
    {
        public override GetMasterDataSearchQuyTrinhParameter ToParameter()
        {
            return new GetMasterDataSearchQuyTrinhParameter()
            {
                UserId = UserId
            };
        }
    }
}
