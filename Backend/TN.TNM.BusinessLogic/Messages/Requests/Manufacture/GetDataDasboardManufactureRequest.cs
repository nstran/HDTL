using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetDataDasboardManufactureRequest : BaseRequest<GetDataDasboardManufactureParameter>
    {
        public override GetDataDasboardManufactureParameter ToParameter()
        {
            return new GetDataDasboardManufactureParameter()
            {
               UserId = UserId
            };
        }
    }
}
